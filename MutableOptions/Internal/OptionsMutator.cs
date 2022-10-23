namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options.Mutable.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    internal sealed class OptionsMutator<TOptions> : IOptionsMutator<TOptions>
        where TOptions : class
    {
        private readonly IOptionsMonitor<TOptions> _optionsMonitor;
        private readonly IOptionsMonitorCache<TOptions> _cache;
        private readonly IEnumerable<IOptionsMutatorChangeTokenSource<TOptions>> _mutatorChangeTokenSources;
        private readonly IEnumerable<INamedOptionsConfiguration<TOptions>> _configurations;
        private readonly IEnumerable<INamedOptionsMutatorConfiguration<TOptions>> _mutatorConfigurations;

        public OptionsMutator(
            IOptionsMonitor<TOptions> optionsMonitor,
            IOptionsMonitorCache<TOptions> cache,
            IEnumerable<IOptionsMutatorChangeTokenSource<TOptions>> mutatorChangeTokenSources,
            IEnumerable<INamedOptionsConfiguration<TOptions>> configurations,
            IEnumerable<INamedOptionsMutatorConfiguration<TOptions>> mutatorConfigurations)
        {
            Argument.NotNull(optionsMonitor);
            Argument.NotNull(cache);
            Argument.NotNull(mutatorChangeTokenSources);
            Argument.NotNull(configurations);
            Argument.NotNull(mutatorConfigurations);

            _optionsMonitor = optionsMonitor;
            _cache = cache;
            _mutatorChangeTokenSources = mutatorChangeTokenSources;
            _configurations = configurations;
            _mutatorConfigurations = mutatorConfigurations;
        }

        public bool Mutate(string? name, Func<TOptions, TOptions> mutator)
        {
            Argument.NotNull(mutator);

            name ??= Options.DefaultName;

            var oldValue = _optionsMonitor.Get(name);
            var newValue = mutator(oldValue);

            var mutatorConfiguration = GetMutatorConfiguration(name);

            var equalityComparer = mutatorConfiguration?.EqualityComparer ?? EqualityComparer<TOptions>.Default;
            if (equalityComparer.Equals(oldValue, newValue))
            {
                return false;
            }

            var isChanged = false;

            var binderOptions = new BinderOptions();
            mutatorConfiguration?.ConfigureBinderOptions(binderOptions);

            var properties = OptionsPropertiesCache<TOptions>.EnumerateProperties(binderOptions.BindNonPublicProperties);
            foreach (var (propertyInfo, getter) in properties)
            {
                var oldPropertyValue = getter(oldValue);
                var newPropertyValue = getter(newValue);

                if (oldPropertyValue is null)
                {
                    if (newPropertyValue is not null)
                    {
                        UpdateConfigurationValue(propertyInfo.Name, newPropertyValue, name);
                        isChanged = true;
                    }

                    continue;
                }

                if (!oldPropertyValue.Equals(newPropertyValue))
                {
                    UpdateConfigurationValue(propertyInfo.Name, newPropertyValue, name);
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                foreach (var optionsMutatorChangeTokenSource in _mutatorChangeTokenSources)
                {
                    if (optionsMutatorChangeTokenSource.Name == name)
                    {
                        optionsMutatorChangeTokenSource.OnMutated();
                    }
                }
            }

            return isChanged;
        }

        private void UpdateConfigurationValue(string propertyName, object? newPropertyValue, string optionsName)
        {
            var value = SerializeValue(newPropertyValue);

            _cache.TryRemove(optionsName);

            foreach (var namedOptionsConfiguration in _configurations)
            {
                if (namedOptionsConfiguration.Name == optionsName)
                {
                    namedOptionsConfiguration.Configuration[propertyName] = value;
                }
            }
        }

        private INamedOptionsMutatorConfiguration<TOptions>? GetMutatorConfiguration(string name)
        {
            return _mutatorConfigurations.FirstOrDefault(comparer => comparer.Name == name);
        }

        private static string? SerializeValue(object? value)
        {
            return value switch
            {
                null => null,
                IConvertible convertible => convertible.ToString(CultureInfo.InvariantCulture),
                _ => value.ToString(),
            };
        }
    }
}
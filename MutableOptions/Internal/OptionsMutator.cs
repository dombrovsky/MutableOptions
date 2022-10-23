namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Options.Mutable.Annotations;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    internal sealed class OptionsMutator<TOptions> : IOptionsMutator<TOptions>
        where TOptions : class
    {
        private readonly IOptionsMonitor<TOptions> _optionsMonitor;
        private readonly IOptionsMonitorCache<TOptions> _cache;
        private readonly IEnumerable<IOptionsMutatorChangeTokenSource<TOptions>> _mutatorChangeTokenSources;
        private readonly IEnumerable<INamedOptionsConfiguration<TOptions>> _configurations;
        private readonly IEnumerable<INamedOptionsEqualityComparer<TOptions>> _equalityComparers;

        public OptionsMutator(
            IOptionsMonitor<TOptions> optionsMonitor,
            IOptionsMonitorCache<TOptions> cache,
            IEnumerable<IOptionsMutatorChangeTokenSource<TOptions>> mutatorChangeTokenSources,
            IEnumerable<INamedOptionsConfiguration<TOptions>> configurations,
            IEnumerable<INamedOptionsEqualityComparer<TOptions>> equalityComparers)
        {
            Argument.NotNull(optionsMonitor);
            Argument.NotNull(cache);
            Argument.NotNull(mutatorChangeTokenSources);
            Argument.NotNull(configurations);
            Argument.NotNull(equalityComparers);

            _optionsMonitor = optionsMonitor;
            _cache = cache;
            _mutatorChangeTokenSources = mutatorChangeTokenSources;
            _configurations = configurations;
            _equalityComparers = equalityComparers;
        }

        public bool Mutate(string? name, Func<TOptions, TOptions> mutator)
        {
            Argument.NotNull(mutator);

            name ??= Options.DefaultName;

            var oldValue = _optionsMonitor.Get(name);
            var newValue = mutator(oldValue);

            var equalityComparer = GetEqualityComparer(name);
            if (equalityComparer.Equals(oldValue, newValue))
            {
                return false;
            }

            var properties = typeof(TOptions).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in properties)
            {
                var oldPropertyValue = propertyInfo.GetValue(oldValue);
                var newPropertyValue = propertyInfo.GetValue(newValue);

                if (oldPropertyValue is null)
                {
                    if (newPropertyValue is not null)
                    {
                        UpdateConfigurationValue(propertyInfo, newPropertyValue, name);
                    }

                    continue;
                }

                if (!oldPropertyValue.Equals(newPropertyValue))
                {
                    UpdateConfigurationValue(propertyInfo, newPropertyValue, name);
                }
            }

            return true;
        }

        private void UpdateConfigurationValue(PropertyInfo propertyInfo, object? newPropertyValue, string optionsName)
        {
            var value = SerializeValue(newPropertyValue);

            _cache.TryRemove(optionsName);

            foreach (var namedOptionsConfiguration in _configurations)
            {
                if (namedOptionsConfiguration.Name == optionsName)
                {
                    namedOptionsConfiguration.Configuration[propertyInfo.Name] = value;
                }
            }

            foreach (var optionsMutatorChangeTokenSource in _mutatorChangeTokenSources)
            {
                if (optionsMutatorChangeTokenSource.Name == optionsName)
                {
                    optionsMutatorChangeTokenSource.OnMutated();
                }
            }
        }

        private IEqualityComparer<TOptions> GetEqualityComparer(string name)
        {
            return _equalityComparers.FirstOrDefault(comparer => comparer.Name == name)?.EqualityComparer ?? EqualityComparer<TOptions>.Default;
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
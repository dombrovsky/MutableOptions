namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options.Mutable.Annotations;
    using System;
    using System.Collections.Generic;

    internal sealed class NamedOptionsMutatorConfiguration<TOptions> : INamedOptionsMutatorConfiguration<TOptions>
    {
        private readonly Action<BinderOptions> _configureBinder;

        public NamedOptionsMutatorConfiguration(string? name, IEqualityComparer<TOptions> equalityComparer, Action<BinderOptions> configureBinder)
        {
            Argument.NotNull(equalityComparer);
            Argument.NotNull(configureBinder);

            _configureBinder = configureBinder;

            Name = name ?? Options.DefaultName;
            EqualityComparer = equalityComparer;
        }

        public string Name { get; }

        public IEqualityComparer<TOptions> EqualityComparer { get; }

        public void ConfigureBinderOptions(BinderOptions binderOptions)
        {
            _configureBinder(binderOptions);
        }
    }
}
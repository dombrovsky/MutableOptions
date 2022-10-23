namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Options.Mutable.Annotations;
    using System.Collections.Generic;

    internal sealed class NamedOptionsEqualityComparer<TOptions> : INamedOptionsEqualityComparer<TOptions>
    {
        public NamedOptionsEqualityComparer(string? name, IEqualityComparer<TOptions> equalityComparer)
        {
            Argument.NotNull(equalityComparer);

            Name = name ?? Options.DefaultName;
            EqualityComparer = equalityComparer;
        }

        public string Name { get; }
        public IEqualityComparer<TOptions> EqualityComparer { get; }
    }
}
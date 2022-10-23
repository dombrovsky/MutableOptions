namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using System.Collections.Generic;

    internal interface INamedOptionsEqualityComparer<in TOptions>
    {
        string Name { get; }

        IEqualityComparer<TOptions> EqualityComparer { get; }
    }
}
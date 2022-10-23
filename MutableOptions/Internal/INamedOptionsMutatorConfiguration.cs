namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    internal interface INamedOptionsMutatorConfiguration<in TOptions>
    {
        string Name { get; }

        IEqualityComparer<TOptions> EqualityComparer { get; }

        void ConfigureBinderOptions(BinderOptions binderOptions);
    }
}
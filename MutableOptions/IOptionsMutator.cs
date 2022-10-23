namespace Microsoft.Extensions.Options.Mutable
{
    using System;

    public interface IOptionsMutator<TOptions>
        where TOptions : class
    {
        bool Mutate(string? name, Func<TOptions, TOptions> mutator);
    }
}
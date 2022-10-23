namespace Microsoft.Extensions.Options.Mutable
{
    using Microsoft.Extensions.Options.Mutable.Annotations;
    using System;

    public static class OptionsMutatorExtensions
    {
        public static bool Mutate<TOptions>(this IOptionsMutator<TOptions> optionsMutator, Func<TOptions, TOptions> mutator)
            where TOptions : class
        {
            Argument.NotNull(optionsMutator);

            return optionsMutator.Mutate(Options.DefaultName, mutator);
        }
    }
}
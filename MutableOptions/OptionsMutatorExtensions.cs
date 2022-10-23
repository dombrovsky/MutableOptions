namespace Microsoft.Extensions.Options.Mutable
{
    using Microsoft.Extensions.Options.Mutable.Annotations;
    using System;

    public static class OptionsMutatorExtensions
    {
        /// <summary>
        /// Executes mutation on <typeparamref name="TOptions"/> instance and if changed updates values in the underlying <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
        /// </summary>
        /// <param name="optionsMutator">Mutator instance.</param>
        /// <param name="mutator">Function that returns mutated <typeparamref name="TOptions"/> instance based on original instance.</param>
        /// <returns>True, if values have been updated, otherwise False.</returns>
        public static bool Mutate<TOptions>(this IOptionsMutator<TOptions> optionsMutator, Func<TOptions, TOptions> mutator)
            where TOptions : class
        {
            Argument.NotNull(optionsMutator);

            return optionsMutator.Mutate(Options.DefaultName, mutator);
        }
    }
}
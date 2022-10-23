namespace Microsoft.Extensions.Options.Mutable
{
    using System;

    /// <summary>
    /// Used for mutating <typeparamref name="TOptions"/> instances and storing changes to underlying <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    public interface IOptionsMutator<TOptions>
        where TOptions : class
    {
        /// <summary>
        /// Executes mutation on <typeparamref name="TOptions"/> instance and if changed updates values in the underlying <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the options instance. <see cref="Options.DefaultName"/> is used when null.</param>
        /// <param name="mutator">Function that returns mutated <typeparamref name="TOptions"/> instance based on original instance.</param>
        /// <returns>True, if values have been updated, otherwise False.</returns>
        bool Mutate(string? name, Func<TOptions, TOptions> mutator);
    }
}
namespace Microsoft.Extensions.Options.Mutable
{
    /// <summary>
    /// Combines functionality of <see cref="IOptionsMonitor{TOptions}"/> and <see cref="IOptionsMutator{TOptions}"/>./>
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    public interface IMutableOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>, IOptionsMutator<TOptions>
        where TOptions : class
    {
    }
}
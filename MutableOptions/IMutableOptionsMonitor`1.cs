namespace Microsoft.Extensions.Options.Mutable
{
    public interface IMutableOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>, IOptionsMutator<TOptions>
        where TOptions : class
    {
    }
}
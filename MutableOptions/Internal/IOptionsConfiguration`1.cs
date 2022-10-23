namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Configuration;

    internal interface IOptionsConfiguration<TOptions>
    {
        IConfiguration Configuration { get; }
    }
}

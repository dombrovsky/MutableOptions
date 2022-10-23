namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Configuration;

    internal interface INamedOptionsConfiguration<TOptions>
    {
        string Name { get; }

        IConfiguration Configuration { get; }
    }
}
namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options.Mutable.Annotations;

    internal sealed class NamedOptionsConfiguration<TOptions> : INamedOptionsConfiguration<TOptions>
    {
        public NamedOptionsConfiguration(string? name, IConfiguration configuration)
        {
            Argument.NotNull(configuration);

            Name = name ?? Options.DefaultName;
            Configuration = configuration;
        }

        public string Name { get; }

        public IConfiguration Configuration { get; }
    }
}
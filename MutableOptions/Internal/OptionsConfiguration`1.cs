namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options.Mutable.Annotations;

    internal sealed class OptionsConfiguration<TOptions> : IOptionsConfiguration<TOptions>
    {
        public OptionsConfiguration(IConfiguration configuration)
        {
            Argument.NotNull(configuration);

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
    }
}
namespace MutableOptions.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options.Mutable;
    using MutableOptions.Tests.TestOptions;
    using NUnit.Framework;

    public partial class OptionsMutatorTests
    {
        [Test]
        public void SerializeHierarchy_ShouldNotThrow()
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>(nameof(HierarchicalOptions.StringValue), "foobar"),
                new KeyValuePair<string, string>($"{nameof(HierarchicalOptions.StringValues)}:0", "foo"),
                new KeyValuePair<string, string>($"{nameof(HierarchicalOptions.StringValues)}:1", "bar"),
                new KeyValuePair<string, string>($"{nameof(HierarchicalOptions.InnerOptions)}:Foo:{nameof(SimpleOptions.StringValue)}", "foo"),
                new KeyValuePair<string, string>($"{nameof(HierarchicalOptions.InnerOptions)}:Bar:{nameof(SimpleOptions.StringValue)}", "bar"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<HierarchicalOptions>(context.Configuration);
                })
                .Build();

            var optionsMonitor = CreateOptionsMonitor<HierarchicalOptions>(host.Services);
            var optionsMutator = CreateSut<HierarchicalOptions>(host.Services);

            Assert.That(() =>
                {
                    optionsMutator.Mutate(options => options with
                    {
                        InnerOptions = new Dictionary<string, SimpleOptions>
                        {
                            { "2", new SimpleOptions { StringValue = "3" } }
                        },
                    });
                },
                Throws.Nothing);
        }
    }
}
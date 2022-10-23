namespace MutableOptions.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options.Mutable;
    using MutableOptions.Tests.TestOptions;
    using NUnit.Framework;

    public partial class OptionsMutatorTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void CanMutatePrivateProperty_IfConfigured(bool bindNonPublicProperties)
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("PrivateStringValue", "foobar"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<NonPublicOptions>(context.Configuration, binder => binder.BindNonPublicProperties = bindNonPublicProperties);
                })
                .Build();

            var optionsMonitor = CreateOptionsMonitor<NonPublicOptions>(host.Services);
            var optionsMutator = CreateSut<NonPublicOptions>(host.Services);

            var mutationResult = optionsMutator.Mutate(options => NonPublicOptions.Create("foobar_new"));

            Assert.That(mutationResult, Is.EqualTo(bindNonPublicProperties));
            if (bindNonPublicProperties)
            {
                Assert.That(optionsMonitor.CurrentValue.GetPrivateStringValue(), Is.EqualTo("foobar_new"));
            }
            else
            {
                Assert.That(optionsMonitor.CurrentValue.GetPrivateStringValue(), Is.Empty);
            }
        }
    }
}
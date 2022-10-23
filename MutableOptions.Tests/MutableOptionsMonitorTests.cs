namespace MutableOptions.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Options.Mutable;
    using MutableOptions.Tests.TestOptions;
    using NUnit.Framework;

    [TestFixture]
    public class MutableOptionsMonitorTests : OptionsMutatorTests
    {
        protected override IOptionsMutator<T> CreateSut<T>(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IMutableOptionsMonitor<T>>();
        }

        protected override IOptionsMonitor<T> CreateOptionsMonitor<T>(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IMutableOptionsMonitor<T>>();
        }

        [Test]
        public void NotifyPropertyChangedWrapper_ReadingCurrentValueFromChangedCallback_ShouldReturnNewValue()
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("StringValue", "foo"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<SimpleOptions>(context.Configuration);
                    services.AddSingleton<SimpleSettingsService>();
                })
                .Build();

            var simpleSettingsService = host.Services.GetRequiredService<SimpleSettingsService>();
            Assert.That(simpleSettingsService.StringValue, Is.Not.EqualTo("bar"));

            var onChangeCalledTimes = 0;
            simpleSettingsService.PropertyChanged += (_, _) =>
            {
                Assert.That(simpleSettingsService.StringValue, Is.EqualTo("bar"));
                onChangeCalledTimes++;
            };

            simpleSettingsService.StringValue = "bar";

            Assert.That(onChangeCalledTimes, Is.EqualTo(1));
        }
    }
}
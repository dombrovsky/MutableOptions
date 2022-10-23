namespace MutableOptions.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Options.Mutable;
    using MutableOptions.Tests.TestOptions;
    using NUnit.Framework;

    public partial class OptionsMutatorTests
    {
        [TestCase("Foo")]
        [TestCase("Bar")]
        [TestCase("")]
        public void NamedOptions_WhenOneMutated_OtherShouldHaveOldValue(string name)
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("Foo:StringValue", "foo"),
                new KeyValuePair<string, string>("Bar:StringValue", "bar"),
                new KeyValuePair<string, string>("StringValue", "foobar"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<SimpleOptions>("Foo", context.Configuration.GetSection("Foo"), _equalityComparer);
                    services.ConfigureMutable<SimpleOptions>("Bar", context.Configuration.GetSection("Bar"), _equalityComparer);
                    services.ConfigureMutable<SimpleOptions>(context.Configuration, _equalityComparer);
                })
                .Build();

            var optionsMonitor = CreateOptionsMonitor<SimpleOptions>(host.Services);
            var optionsMutator = CreateSut<SimpleOptions>(host.Services);

            optionsMutator.Mutate(name, options => options with { StringValue = $"{name}_new" });

            Assert.That(optionsMonitor.Get(name).StringValue, Is.EqualTo($"{name}_new"));
            foreach (var keyValuePair in configData)
            {
                var keyParts = keyValuePair.Key.Split(':');
                var keyName = keyParts.Length > 1 ? keyParts[0] : Options.DefaultName;

                if (keyName != name)
                {
                    Assert.That(optionsMonitor.Get(keyName).StringValue, Is.EqualTo(keyValuePair.Value));
                }
            }
        }

        [TestCase("Foo")]
        [TestCase("Bar")]
        [TestCase("")]
        public void NamedOptions_WhenOneMutated_OptionsMonitorShouldRaiseNamedOnChange(string name)
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("Foo:StringValue", "foo"),
                new KeyValuePair<string, string>("Bar:StringValue", "bar"),
                new KeyValuePair<string, string>("StringValue", "foo"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<SimpleOptions>("Foo", context.Configuration.GetSection("Features:Foo"), _equalityComparer);
                    services.ConfigureMutable<SimpleOptions>("Bar", context.Configuration.GetSection("Features:Bar"), _equalityComparer);
                    services.ConfigureMutable<SimpleOptions>(context.Configuration, _equalityComparer);
                })
                .Build();

            var optionsMonitor = CreateOptionsMonitor<SimpleOptions>(host.Services);
            var changeNotifications = new List<(SimpleOptions Options, string Name)>();
            using var onChangeSubscription = optionsMonitor.OnChange((foo, s) => changeNotifications.Add((foo, s)));

            var optionsMutator = CreateSut<SimpleOptions>(host.Services);

            optionsMutator.Mutate(name, options => options with { StringValue = $"{name}_new" });

            Assert.That(changeNotifications, Is.EqualTo(new [] { (new SimpleOptions { StringValue = $"{name}_new" }, name) }));
        }
    }
}
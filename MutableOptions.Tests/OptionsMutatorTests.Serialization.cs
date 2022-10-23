namespace MutableOptions.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options.Mutable;
    using MutableOptions.Tests.TestOptions;
    using NUnit.Framework;
    using System.Globalization;

    public partial class OptionsMutatorTests
    {
        [Test]
        public void SerializePrimitiveTypes()
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>(nameof(ConvertibleOptions.IntValue), "0"),
                new KeyValuePair<string, string>(nameof(ConvertibleOptions.DateTimeValue), DateTime.UnixEpoch.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>(nameof(ConvertibleOptions.DateTimeOffsetValue), DateTimeOffset.UnixEpoch.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>(nameof(ConvertibleOptions.TimeSpanValue), TimeSpan.FromDays(1.111).ToString()),
                new KeyValuePair<string, string>(nameof(ConvertibleOptions.EnumValue), "Foo"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<ConvertibleOptions>(context.Configuration);
                })
                .Build();

            var optionsMonitor = CreateOptionsMonitor<ConvertibleOptions>(host.Services);
            var optionsMutator = CreateSut<ConvertibleOptions>(host.Services);

            optionsMutator.Mutate(options => options with
            {
                IntValue = 42,
                DateTimeValue = DateTime.UnixEpoch.AddDays(1),
            });

            optionsMutator.Mutate(options => options with
            {
                DateTimeOffsetValue = DateTimeOffset.UnixEpoch.AddDays(1),
                TimeSpanValue = TimeSpan.FromDays(2.222),
                EnumValue = MyEnum.Bar,
            });

            Assert.That(optionsMonitor.CurrentValue.IntValue, Is.EqualTo(42));
            Assert.That(optionsMonitor.CurrentValue.DateTimeValue, Is.EqualTo(DateTime.UnixEpoch.AddDays(1)));
            Assert.That(optionsMonitor.CurrentValue.DateTimeOffsetValue, Is.EqualTo(DateTimeOffset.UnixEpoch.AddDays(1)));
            Assert.That(optionsMonitor.CurrentValue.TimeSpanValue, Is.EqualTo(TimeSpan.FromDays(2.222)));
            Assert.That(optionsMonitor.CurrentValue.EnumValue, Is.EqualTo(MyEnum.Bar));
        }
    }
}
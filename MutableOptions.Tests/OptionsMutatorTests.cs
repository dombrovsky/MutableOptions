namespace MutableOptions.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Options.Mutable;
    using MutableOptions.Tests.TestOptions;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public partial class OptionsMutatorTests
    {
        private IEqualityComparer<SimpleOptions> _equalityComparer;

        [SetUp]
        public void Setup()
        {
            _equalityComparer = Substitute.For<IEqualityComparer<SimpleOptions>>();
            _equalityComparer
                .Equals(Arg.Any<SimpleOptions?>(), Arg.Any<SimpleOptions>())
                .Returns(info => EqualityComparer<SimpleOptions>.Default.Equals(info.ArgAt<SimpleOptions?>(0), info.ArgAt<SimpleOptions?>(1)));
        }

        [Test]
        public void OptionsMonitor_ReadingCurrentValueFromChangedCallback_ShouldReturnNewValue()
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("StringValue", "foo"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) => services.ConfigureMutable<SimpleOptions>(context.Configuration))
                .Build();

            var optionsMonitor = CreateOptionsMonitor<SimpleOptions>(host.Services);
            Assert.That(optionsMonitor.CurrentValue.StringValue, Is.Not.EqualTo("bar"));

            var onChangeCalledTimes = 0;
            optionsMonitor.OnChange(options =>
            {
                Assert.That(optionsMonitor.CurrentValue.StringValue, Is.EqualTo("bar").And.EqualTo(options.StringValue));
                onChangeCalledTimes++;
            });

            var optionsMutator = CreateSut<SimpleOptions>(host.Services);
            optionsMutator.Mutate(options => options with { StringValue = "bar" });

            Assert.That(onChangeCalledTimes, Is.EqualTo(1));
        }

        [Test]
        public void ConfigurationEmpty_WhenMutate_ShouldWriteToConfiguration()
        {
            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection())
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<SimpleOptions>(context.Configuration);
                })
                .Build();

            var optionsMonitor = CreateOptionsMonitor<SimpleOptions>(host.Services);
            Assert.That(optionsMonitor.CurrentValue.StringValue, Is.EqualTo(string.Empty));

            var optionsMutator = CreateSut<SimpleOptions>(host.Services);
            optionsMutator.Mutate(options => options with { StringValue = "foobar" });

            Assert.That(optionsMonitor.CurrentValue.StringValue, Is.EqualTo("foobar"));
            Assert.That(host.Services.GetRequiredService<IConfiguration>().GetValue("StringValue", "default"), Is.EqualTo("foobar"));
        }

        [Test]
        public void NullablePropertyHasValue_WhenMutateToNull_ShouldRemoveFromConfiguration()
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("StringValue", "foobar"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<NullableOptions>(context.Configuration);
                })
                .Build();

            var optionsMonitor = CreateOptionsMonitor<NullableOptions>(host.Services);
            Assert.That(optionsMonitor.CurrentValue.StringValue, Is.EqualTo("foobar"));

            var optionsMutator = CreateSut<NullableOptions>(host.Services);
            optionsMutator.Mutate(options => options with { StringValue = null });

            Assert.That(optionsMonitor.CurrentValue.StringValue, Is.Null);
            Assert.That(host.Services.GetRequiredService<IConfiguration>().GetValue("StringValue", "default"), Is.EqualTo("default"));
        }

        [Test]
        public void WhenDifferentValueAfterMutation_OptionsSnapshotShouldHaveNewValue()
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("StringValue", "foobar"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable(context.Configuration, _equalityComparer);
                })
                .Build();

            var oldValue = host.Services.GetRequiredService<IOptionsSnapshot<SimpleOptions>>().Value.StringValue;

            var optionsMutator = CreateSut<SimpleOptions>(host.Services);
            optionsMutator.Mutate(options => options with { StringValue = "foobar_new" });

            Assert.That(host.Services.CreateScope().ServiceProvider.GetRequiredService<IOptionsSnapshot<SimpleOptions>>().Value.StringValue, Is.EqualTo("foobar_new").And.Not.EqualTo(oldValue));
        }

        [Test]
        public void OptionsInitialized_WhenDifferentValueAfterMutation_OptionsShouldHaveOldValue()
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("StringValue", "foobar"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable(context.Configuration, _equalityComparer);
                })
                .Build();

            var oldValue = host.Services.GetRequiredService<IOptions<SimpleOptions>>().Value.StringValue;

            var optionsMutator = CreateSut<SimpleOptions>(host.Services);
            optionsMutator.Mutate(options => options with { StringValue = "foobar_new" });

            Assert.That(host.Services.GetRequiredService<IOptions<SimpleOptions>>().Value.StringValue, Is.EqualTo(oldValue));
        }

        [Test]
        public void WhenDifferentValueAfterMutation_OptionsMonitorShouldUpdateValue()
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("StringValue", "foobar"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<SimpleOptions>(context.Configuration, _equalityComparer);
                })
                .Build();

            var optionsMonitor = CreateOptionsMonitor<SimpleOptions>(host.Services);
            var changeNotifications = new List<(SimpleOptions Options, string Name)>();
            using var onChangeSubscription = optionsMonitor.OnChange((foo, s) => changeNotifications.Add((foo, s)));
            var optionsMutator = CreateSut<SimpleOptions>(host.Services);

            var previousValue = optionsMonitor.CurrentValue;
            
            var mutationResult = optionsMutator.Mutate(options => options with { StringValue = "foobar_new" });

            Assert.That(mutationResult, Is.True);
            Assert.That(optionsMonitor.CurrentValue, Is.Not.SameAs(previousValue));
            Assert.That(optionsMonitor.CurrentValue.StringValue, Is.EqualTo("foobar_new"));
            Assert.That(changeNotifications, Is.EqualTo(new[] { (new SimpleOptions { StringValue = "foobar_new" }, Options.DefaultName) }));
        }

        [Test]
        public void WhenSameValueAfterMutation_OptionsMonitorShouldNotUpdateValue()
        {
            var configData = new[]
            {
                new KeyValuePair<string, string>("StringValue", "foobar"),
            };

            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(configData))
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureMutable<SimpleOptions>(context.Configuration, _equalityComparer);
                })
                .Build();

            var optionsMonitor = CreateOptionsMonitor<SimpleOptions>(host.Services);
            var changeNotifications = new List<(SimpleOptions Options, string Name)>();
            using var onChangeSubscription = optionsMonitor.OnChange((foo, s) => changeNotifications.Add((foo, s)));
            var optionsMutator = CreateSut<SimpleOptions>(host.Services);

            var previousValue = optionsMonitor.CurrentValue;

            _equalityComparer.Equals(Arg.Any<SimpleOptions?>(), Arg.Any<SimpleOptions>()).Returns(true);
            var mutationResult = optionsMutator.Mutate(options => options with { StringValue = "foobar_new" });

            Assert.That(mutationResult, Is.False);
            Assert.That(optionsMonitor.CurrentValue, Is.SameAs(previousValue));
            Assert.That(changeNotifications, Is.Empty);
        }
        
        protected virtual IOptionsMutator<T> CreateSut<T>(IServiceProvider serviceProvider)
            where T : class
        {
            return serviceProvider.GetRequiredService<IOptionsMutator<T>>();
        }

        protected virtual IOptionsMonitor<T> CreateOptionsMonitor<T>(IServiceProvider serviceProvider)
            where T : class
        {
            return serviceProvider.GetRequiredService<IOptionsMonitor<T>>();
        }
    }
}
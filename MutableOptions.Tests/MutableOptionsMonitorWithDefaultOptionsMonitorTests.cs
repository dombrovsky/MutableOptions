namespace MutableOptions.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options.Mutable;
    using NUnit.Framework;

    [TestFixture]
    public class MutableOptionsMonitorWithDefaultOptionsMonitorTests : OptionsMutatorTests
    {
        protected override IOptionsMutator<T> CreateSut<T>(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IMutableOptionsMonitor<T>>();
        }
    }
}
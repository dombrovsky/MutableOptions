namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using System;

    internal sealed class MutableOptionsMonitor<TOptions> : IMutableOptionsMonitor<TOptions>
        where TOptions : class
    {
        private readonly IOptionsMonitor<TOptions> _optionsMonitor;
        private readonly IOptionsMutator<TOptions> _optionsMutator;

        public MutableOptionsMonitor(IOptionsMonitor<TOptions> optionsMonitor, IOptionsMutator<TOptions> optionsMutator)
        {
            _optionsMonitor = optionsMonitor;
            _optionsMutator = optionsMutator;
        }

        public TOptions CurrentValue => _optionsMonitor.CurrentValue;

        public TOptions Get(string name)
        {
            return _optionsMonitor.Get(name);
        }

        public IDisposable OnChange(Action<TOptions, string> listener)
        {
            return _optionsMonitor.OnChange(listener);
        }

        public bool Mutate(string? name, Func<TOptions, TOptions> mutator)
        {
            return _optionsMutator.Mutate(name, mutator);
        }
    }
}
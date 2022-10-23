namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using System.Threading;

    internal sealed class OptionsMutatorChangeTokenSource<TOptions> : IOptionsMutatorChangeTokenSource<TOptions>
    {
        private ConfigurationReloadToken _changeToken;

        public OptionsMutatorChangeTokenSource(string name)
        {
            Name = name;

            _changeToken = new ConfigurationReloadToken();
        }

        public string Name { get; }

        public IChangeToken GetChangeToken()
        {
            return _changeToken;
        }

        public void OnMutated()
        {
            var previousToken = Interlocked.Exchange(ref _changeToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }
    }
}
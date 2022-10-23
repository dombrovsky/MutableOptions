namespace MutableOptions.Tests.TestOptions
{
    using Microsoft.Extensions.Options.Mutable;
    using System.ComponentModel;

    internal sealed class SimpleSettingsService : INotifyPropertyChanged, IDisposable
    {
        private readonly IMutableOptionsMonitor<SimpleOptions> _mutableOptionsMonitor;
        private readonly IDisposable _changeSubscription;

        public SimpleSettingsService(IMutableOptionsMonitor<SimpleOptions> mutableOptionsMonitor)
        {
            _mutableOptionsMonitor = mutableOptionsMonitor;
            _changeSubscription = _mutableOptionsMonitor.OnChange(HandleSimpleOptionsChanged);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string StringValue
        {
            get => _mutableOptionsMonitor.CurrentValue.StringValue;
            set => _mutableOptionsMonitor.Mutate(options => options with { StringValue = value });
        }

        public void Dispose()
        {
            _changeSubscription.Dispose();
        }

        private void HandleSimpleOptionsChanged(SimpleOptions simpleOptions, string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
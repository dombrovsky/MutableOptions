namespace MutableOptions.Tests.TestOptions
{
    internal sealed record NonPublicOptions
    {
        private string PrivateStringValue { get; init; } = string.Empty;

        public string GetPrivateStringValue()
        {
            return PrivateStringValue;
        }

        public static NonPublicOptions Create(string privateStringValue)
        {
            return new NonPublicOptions { PrivateStringValue = privateStringValue };
        }
    }
}
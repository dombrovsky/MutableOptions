namespace MutableOptions.Tests.TestOptions
{
    internal sealed record SimpleOptions
    {
        public string StringValue { get; init; } = string.Empty;
    }
}
namespace MutableOptions.Tests.TestOptions
{
    internal sealed record HierarchicalOptions : SimpleOptions
    {
        public Dictionary<string, SimpleOptions> InnerOptions { get; init; } = new Dictionary<string, SimpleOptions>();

        public List<string> StringValues { get; init; } = new List<string>();
    }
}
namespace MutableOptions.Tests.TestOptions
{
    public enum MyEnum
    {
        None = 0,
        Foo,
        Bar,
    }

    internal sealed record ConvertibleOptions
    {
        public int IntValue { get; init; }

        public DateTime DateTimeValue { get; init; }

        public DateTimeOffset DateTimeOffsetValue { get; init; }

        public TimeSpan TimeSpanValue { get; init; }

        public MyEnum EnumValue { get; init; }
    }
}
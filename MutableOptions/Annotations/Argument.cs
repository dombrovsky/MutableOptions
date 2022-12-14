namespace Microsoft.Extensions.Options.Mutable.Annotations
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal static class Argument
    {
        public static void NotNull([NotNull] object? argument, [CallerArgumentExpression("argument")] string? parameterName = null)
        {
#if NET6_0
            ArgumentNullException.ThrowIfNull(argument, parameterName);
#else
            if (argument is null)
            {
                throw new ArgumentNullException(parameterName);
            }
#endif
        }

        public static void Assert<T>(in T argument, Func<T, bool> predicate, string message, [CallerArgumentExpression("argument")] string? parameterName = null)
        {
            Argument.NotNull(predicate);
            Argument.NotNull(message);

            if (!predicate(argument))
            {
                throw new ArgumentException(message, parameterName);
            }
        }
    }
}

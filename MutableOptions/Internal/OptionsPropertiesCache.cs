namespace Microsoft.Extensions.Options.Mutable.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class OptionsPropertiesCache<TOptions>
    {
        private static readonly Lazy<IEnumerable<(PropertyInfo Property, Func<TOptions, object?> Getter)>> publicProperties = new(() => BuildProperties(BindingFlags.Instance | BindingFlags.Public));
        private static readonly Lazy<IEnumerable<(PropertyInfo Property, Func<TOptions, object?> Getter)>> publicAndNonPublicProperties = new(() => BuildProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

        public static IEnumerable<(PropertyInfo Property, Func<TOptions, object?> Getter)> EnumerateProperties(bool nonPublic = false)
        {
            return nonPublic ? publicAndNonPublicProperties.Value : publicProperties.Value;
        }

        private static IEnumerable<(PropertyInfo Property, Func<TOptions, object?> Getter)> BuildProperties(BindingFlags bindingFlags)
        {
            return typeof(TOptions)
                .GetProperties(bindingFlags)
                .Select(propertyInfo => (propertyInfo, BuildGetter(propertyInfo)));
        }
        
        private static Func<TOptions, object?> BuildGetter(PropertyInfo propertyInfo)
        {
            return options => propertyInfo.GetValue(options);
        }
    }
}
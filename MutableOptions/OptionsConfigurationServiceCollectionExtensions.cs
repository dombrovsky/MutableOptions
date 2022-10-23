namespace Microsoft.Extensions.Options.Mutable
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options.Mutable.Annotations;
    using Microsoft.Extensions.Options.Mutable.Internal;
    using System;
    using System.Collections.Generic;

    public static class OptionsConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, IConfiguration config)
            where TOptions : class, IEquatable<TOptions>
        {
            return services.ConfigureMutable(Options.DefaultName, config, _ => { }, EqualityComparer<TOptions>.Default);
        }

        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, string name, IConfiguration config)
            where TOptions : class, IEquatable<TOptions>
        {
            return services.ConfigureMutable(name, config, _ => { }, EqualityComparer<TOptions>.Default);
        }

        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, string name, IConfiguration config, Action<BinderOptions> configureBinder)
            where TOptions : class, IEquatable<TOptions>
        {
            return services.ConfigureMutable(name, config, configureBinder, EqualityComparer<TOptions>.Default);
        }

        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, IConfiguration config, Action<BinderOptions> configureBinder)
            where TOptions : class, IEquatable<TOptions>
        {
            return services.ConfigureMutable(Options.DefaultName, config, configureBinder, EqualityComparer<TOptions>.Default);
        }

        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, IConfiguration config, IEqualityComparer<TOptions> equalityComparer)
            where TOptions : class
        {
            return services.ConfigureMutable(Options.DefaultName, config, _ => { }, equalityComparer);
        }

        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, IConfiguration config, Action<BinderOptions> configureBinder, IEqualityComparer<TOptions> equalityComparer)
            where TOptions : class
        {
            return services.ConfigureMutable(Options.DefaultName, config, configureBinder, equalityComparer);
        }

        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, string name, IConfiguration config, IEqualityComparer<TOptions> equalityComparer)
            where TOptions : class
        {
            return services.ConfigureMutable(name, config, _ => { }, equalityComparer);
        }

        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, string name, IConfiguration config, Action<BinderOptions> configureBinder, IEqualityComparer<TOptions> equalityComparer)
            where TOptions : class
        {
            Argument.NotNull(services);
            Argument.NotNull(equalityComparer);

            services.Configure<TOptions>(name, config, configureBinder);

            services.TryAdd(ServiceDescriptor.Singleton(typeof(IMutableOptionsMonitor<>), typeof(MutableOptionsMonitor<>)));
            services.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsMutator<>), typeof(OptionsMutator<>)));

            var mutatorChangeTokenSource = new OptionsMutatorChangeTokenSource<TOptions>(name);
            services.AddSingleton<IOptionsMutatorChangeTokenSource<TOptions>>(mutatorChangeTokenSource);
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(mutatorChangeTokenSource);

            services.AddSingleton<INamedOptionsConfiguration<TOptions>>(new NamedOptionsConfiguration<TOptions>(name, config));
            services.AddSingleton<INamedOptionsMutatorConfiguration<TOptions>>(new NamedOptionsMutatorConfiguration<TOptions>(name, equalityComparer, configureBinder));

            return services;
        }
    }
}
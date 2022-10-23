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
        /// <summary>
        /// Registers a mutable configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <remarks>To mutate use <see cref="IOptionsMutator{TOptions}"/> or <see cref="IMutableOptionsMonitor{TOptions}"/>.</remarks>
        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, IConfiguration config)
            where TOptions : class, IEquatable<TOptions>
        {
            return services.ConfigureMutable(Options.DefaultName, config, _ => { }, EqualityComparer<TOptions>.Default);
        }

        /// <summary>
        /// Registers a mutable configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <remarks>To mutate use <see cref="IOptionsMutator{TOptions}"/> or <see cref="IMutableOptionsMonitor{TOptions}"/>.</remarks>
        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, string name, IConfiguration config)
            where TOptions : class, IEquatable<TOptions>
        {
            return services.ConfigureMutable(name, config, _ => { }, EqualityComparer<TOptions>.Default);
        }

        /// <summary>
        /// Registers a mutable configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <remarks>To mutate use <see cref="IOptionsMutator{TOptions}"/> or <see cref="IMutableOptionsMonitor{TOptions}"/>.</remarks>
        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, string name, IConfiguration config, Action<BinderOptions> configureBinder)
            where TOptions : class, IEquatable<TOptions>
        {
            return services.ConfigureMutable(name, config, configureBinder, EqualityComparer<TOptions>.Default);
        }

        /// <summary>
        /// Registers a mutable configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <remarks>To mutate use <see cref="IOptionsMutator{TOptions}"/> or <see cref="IMutableOptionsMonitor{TOptions}"/>.</remarks>
        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, IConfiguration config, Action<BinderOptions> configureBinder)
            where TOptions : class, IEquatable<TOptions>
        {
            return services.ConfigureMutable(Options.DefaultName, config, configureBinder, EqualityComparer<TOptions>.Default);
        }

        /// <summary>
        /// Registers a mutable configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="equalityComparer">Used to compare whether options had changed after calling <see cref="IOptionsMutator{TOptions}.Mutate"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <remarks>To mutate use <see cref="IOptionsMutator{TOptions}"/> or <see cref="IMutableOptionsMonitor{TOptions}"/>.</remarks>
        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, IConfiguration config, IEqualityComparer<TOptions> equalityComparer)
            where TOptions : class
        {
            return services.ConfigureMutable(Options.DefaultName, config, _ => { }, equalityComparer);
        }

        /// <summary>
        /// Registers a mutable configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <param name="equalityComparer">Used to compare whether options had changed after calling <see cref="IOptionsMutator{TOptions}.Mutate"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <remarks>To mutate use <see cref="IOptionsMutator{TOptions}"/> or <see cref="IMutableOptionsMonitor{TOptions}"/>.</remarks>
        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, IConfiguration config, Action<BinderOptions> configureBinder, IEqualityComparer<TOptions> equalityComparer)
            where TOptions : class
        {
            return services.ConfigureMutable(Options.DefaultName, config, configureBinder, equalityComparer);
        }

        /// <summary>
        /// Registers a mutable configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="equalityComparer">Used to compare whether options had changed after calling <see cref="IOptionsMutator{TOptions}.Mutate"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <remarks>To mutate use <see cref="IOptionsMutator{TOptions}"/> or <see cref="IMutableOptionsMonitor{TOptions}"/>.</remarks>
        public static IServiceCollection ConfigureMutable<TOptions>(this IServiceCollection services, string name, IConfiguration config, IEqualityComparer<TOptions> equalityComparer)
            where TOptions : class
        {
            return services.ConfigureMutable(name, config, _ => { }, equalityComparer);
        }

        /// <summary>
        /// Registers a mutable configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="config">The configuration being bound.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <param name="equalityComparer">Used to compare whether options had changed after calling <see cref="IOptionsMutator{TOptions}.Mutate"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <remarks>To mutate use <see cref="IOptionsMutator{TOptions}"/> or <see cref="IMutableOptionsMonitor{TOptions}"/>.</remarks>
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
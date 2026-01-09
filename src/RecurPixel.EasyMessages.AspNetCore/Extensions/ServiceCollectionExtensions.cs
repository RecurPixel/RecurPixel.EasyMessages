using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore.Configuration;
using RecurPixel.EasyMessages.AspNetCore.Configuration.Validation;
using RecurPixel.EasyMessages.AspNetCore.Interceptors;
using RecurPixel.EasyMessages.Configuration;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Formatters;
using RecurPixel.EasyMessages.Interceptors;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.AspNetCore;

/// <summary>
/// Extension methods for configuring EasyMessages in the dependency injection container
/// </summary>
public static class ServiceCollectionExtensions
{
    #region New IOptions Pattern (Recommended)

    /// <summary>
    /// Adds EasyMessages services with IOptions pattern support
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration instance (for appsettings.json binding)</param>
    /// <param name="configure">Optional programmatic configuration</param>
    /// <returns>The service collection for chaining</returns>
    /// <remarks>
    /// This is the recommended way to configure EasyMessages with full IOptions support.
    ///
    /// Features:
    /// - Configuration from appsettings.json
    /// - Programmatic configuration overrides
    /// - Validation on startup
    /// - Hot reload support (with IOptionsMonitor)
    ///
    /// Usage:
    /// services.AddEasyMessages(configuration);
    /// services.AddEasyMessages(configuration, options => { ... });
    /// services.AddEasyMessages(configuration, EasyMessagesPresets.Production);
    /// </remarks>
    public static IServiceCollection AddEasyMessages(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<EasyMessagesOptions>? configure = null)
    {
        // Bind from appsettings.json
        // Note: Validation happens lazily when options are first accessed, not on startup
        // This allows for testing scenarios with non-existent files
        services.AddOptions<EasyMessagesOptions>()
            .Bind(configuration.GetSection(EasyMessagesOptions.SectionName))
            .ValidateDataAnnotations();

        // Apply programmatic configuration if provided
        if (configure != null)
        {
            services.Configure(configure);
        }

        // Register custom validator
        services.AddSingleton<IValidateOptions<EasyMessagesOptions>, EasyMessagesOptionsValidator>();

        // Configure EasyMessages using resolved options
        services.AddSingleton<EasyMessagesConfigurator>();

        // Trigger configuration on startup
        services.AddHostedService<EasyMessagesStartupService>();

        return services;
    }

    /// <summary>
    /// Adds EasyMessages services with a preset configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <param name="preset">Preset configuration (Development, Production, Testing, etc.)</param>
    /// <returns>The service collection for chaining</returns>
    /// <remarks>
    /// Convenience method for using preset configurations.
    ///
    /// Usage:
    /// services.AddEasyMessages(configuration, EasyMessagesPresets.Development);
    /// services.AddEasyMessages(configuration, EasyMessagesPresets.Production);
    /// </remarks>
    public static IServiceCollection AddEasyMessages(
        this IServiceCollection services,
        IConfiguration configuration,
        EasyMessagesOptions preset)
    {
        return services.AddEasyMessages(configuration, options =>
        {
            // Copy all settings from preset
            options.Formatter = preset.Formatter;
            options.Interceptor = preset.Interceptor;
            options.Logging = preset.Logging;
            options.Storage = preset.Storage;
            options.Localization = preset.Localization;
            options.CustomStores = preset.CustomStores;
            options.CustomFormatters = preset.CustomFormatters;
            options.Interceptors = preset.Interceptors;
        });
    }

    #endregion

    #region Legacy Support (Backward Compatibility)

    /// <summary>
    /// Adds EasyMessages services with legacy MessageConfiguration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action</param>
    /// <returns>The service collection for chaining</returns>
    /// <remarks>
    /// DEPRECATED: This method is maintained for backward compatibility.
    /// Use AddEasyMessages(IConfiguration, Action&lt;EasyMessagesOptions&gt;) instead.
    ///
    /// This method:
    /// - Does not support appsettings.json binding
    /// - Does not support validation
    /// - Does not support hot reload
    ///
    /// Will be removed in a future major version.
    /// </remarks>
    [Obsolete("Use AddEasyMessages(IConfiguration, Action<EasyMessagesOptions>) instead. This method will be removed in version 2.0.")]
    public static IServiceCollection AddEasyMessages(
        this IServiceCollection services,
        Action<MessageConfiguration>? configure = null)
    {
        var options = new MessageConfiguration();
        configure?.Invoke(options);

        // Apply formatter options globally
        FormatterConfiguration.SetDefaultOptions(options.FormatterOptions);

        // Configure stores based on options
        ConfigureStores(options);

        // Configure formatters
        ConfigureFormatters(options);

        // Configure interceptors
        ConfigureInterceptorsLegacy(services, options);

        services.AddSingleton(options);

        return services;
    }

    private static void ConfigureStores(MessageConfiguration options)
    {
        var stores = new List<IMessageStore>();

        // Custom file path
        if (!string.IsNullOrEmpty(options.CustomMessagesPath))
        {
            stores.Add(new FileMessageStore(options.CustomMessagesPath));
        }

        // User-provided stores
        if (options.CustomStores?.Any() == true)
        {
            stores.AddRange(options.CustomStores);
        }

        // Apply to registry
        if (stores.Any())
        {
            MessageRegistry.UseStores(stores.ToArray());
        }
    }

    private static void ConfigureFormatters(MessageConfiguration options)
    {
        if (options.CustomFormatters?.Any() == true)
        {
            foreach (var (name, factory) in options.CustomFormatters)
            {
                FormatterRegistry.Register(name, factory);
            }
        }
    }

    private static void ConfigureInterceptorsLegacy(
        IServiceCollection services,
        MessageConfiguration options)
    {
        if (options.InterceptorOptions.AutoAddCorrelationId)
        {
            services.AddHttpContextAccessor();
        }
        services.AddSingleton<InterceptorInitializer>(sp => new InterceptorInitializer(
            sp,
            options
        ));
    }

    #endregion

    #region Helper Methods (New Pattern)

    /// <summary>
    /// Configure stores from EasyMessagesOptions
    /// </summary>
    private static void ConfigureStores(EasyMessagesOptions options)
    {
        var stores = new List<IMessageStore>();

        // Custom file path
        if (!string.IsNullOrEmpty(options.Storage.CustomMessagesPath))
        {
            stores.Add(new FileMessageStore(options.Storage.CustomMessagesPath));
        }

        // Multiple custom store paths
        if (options.Storage.CustomStorePaths?.Any() == true)
        {
            foreach (var path in options.Storage.CustomStorePaths)
            {
                stores.Add(new FileMessageStore(path));
            }
        }

        // User-provided stores
        if (options.CustomStores?.Any() == true)
        {
            stores.AddRange(options.CustomStores);
        }

        // Apply to registry
        if (stores.Any())
        {
            MessageRegistry.UseStores(stores.ToArray());
        }
    }

    /// <summary>
    /// Configure formatters from EasyMessagesOptions
    /// </summary>
    private static void ConfigureFormatters(EasyMessagesOptions options)
    {
        if (options.CustomFormatters?.Any() == true)
        {
            foreach (var (name, factory) in options.CustomFormatters)
            {
                FormatterRegistry.Register(name, factory);
            }
        }
    }

    /// <summary>
    /// Configure interceptors from EasyMessagesOptions
    /// </summary>
    private static void ConfigureInterceptors(
        IServiceCollection services,
        EasyMessagesOptions options)
    {
        if (options.Interceptor.AutoAddCorrelationId || options.Interceptor.AutoEnrichMetadata)
        {
            services.AddHttpContextAccessor();
        }
    }

    #endregion
}

// Helper class that initializes on first use
internal class InterceptorInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MessageConfiguration _config;
    private bool _initialized;
    private readonly object _lock = new();

    public InterceptorInitializer(IServiceProvider serviceProvider, MessageConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _config = config;
        Initialize();
    }

    private void Initialize()
    {
        lock (_lock)
        {
            if (_initialized)
                return;

            // Auto-logging interceptor
            if (_config.AutoLog)
            {
                var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("EasyMessages");
                InterceptorRegistry.Register(new LoggingInterceptor(() => logger));
            }

            // Correlation ID interceptor (controlled by InterceptorOptions)
            if (_config.InterceptorOptions.AutoAddCorrelationId)
            {
                InterceptorRegistry.Register(
                    new CorrelationIdInterceptor(() =>
                        _serviceProvider.GetRequiredService<IHttpContextAccessor>()
                    )
                );
            }

            // Metadata enrichment interceptor (controlled by InterceptorOptions)
            if (_config.InterceptorOptions.AutoEnrichMetadata)
            {
                InterceptorRegistry.Register(
                    new MetadataEnrichmentInterceptor(
                        () => _serviceProvider.GetRequiredService<IHttpContextAccessor>(),
                        _config.InterceptorOptions.MetadataFields
                    )
                );
            }

            // User-provided interceptors
            if (_config.Interceptors?.Any() == true)
            {
                foreach (var interceptor in _config.Interceptors)
                {
                    InterceptorRegistry.Register(interceptor);
                }
            }

            _initialized = true;
        }
    }
}

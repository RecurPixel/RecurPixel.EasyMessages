using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore.Interceptors;
using RecurPixel.EasyMessages.Configuration;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Formatters;
using RecurPixel.EasyMessages.Interceptors;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEasyMessages(
        this IServiceCollection services,
        Action<MessageConfiguration>? configure = null
    )
    {
        var options = new MessageConfiguration();
        configure?.Invoke(options);

        // Apply formatter options globally
        FormatterConfiguration.SetDefaultOptions(options.FormatterOptions);

        // Configure stores based on options
        ConfigureStores(options);

        // Configure formaters
        ConfigureFormatters(options);

        // Configure interceptors
        ConfigureInterceptors(services, options);

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

    private static void ConfigureInterceptors(
        IServiceCollection services,
        MessageConfiguration options
    )
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

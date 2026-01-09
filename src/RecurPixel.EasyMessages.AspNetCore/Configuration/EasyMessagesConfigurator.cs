using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecurPixel.EasyMessages.AspNetCore.Interceptors;
using RecurPixel.EasyMessages.Configuration;
using RecurPixel.EasyMessages.Core;
using RecurPixel.EasyMessages.Formatters;
using RecurPixel.EasyMessages.Interceptors;
using RecurPixel.EasyMessages.Storage;

namespace RecurPixel.EasyMessages.AspNetCore.Configuration;

/// <summary>
/// Configures EasyMessages based on resolved options
/// </summary>
/// <remarks>
/// This class applies the validated configuration to EasyMessages registries and services.
/// It runs once during application startup after options validation.
/// </remarks>
internal class EasyMessagesConfigurator
{
    private readonly EasyMessagesOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private bool _configured;
    private readonly object _lock = new();

    public EasyMessagesConfigurator(
        IOptions<EasyMessagesOptions> options,
        IServiceProvider serviceProvider
    )
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Applies configuration to EasyMessages
    /// </summary>
    public void Configure()
    {
        lock (_lock)
        {
            if (_configured)
                return;

            // Apply formatter options globally
            FormatterConfiguration.SetDefaultOptions(_options.Formatter);

            // Configure stores
            ConfigureStores();

            // Configure formatters
            ConfigureFormatters();

            // Configure interceptors
            ConfigureInterceptors();

            _configured = true;
        }
    }

    private void ConfigureStores()
    {
        var stores = new List<IMessageStore>();

        // Custom file path
        if (!string.IsNullOrEmpty(_options.Storage.CustomMessagesPath))
        {
            stores.Add(new FileMessageStore(_options.Storage.CustomMessagesPath));
        }

        // Multiple custom store paths
        if (_options.Storage.CustomStorePaths?.Any() == true)
        {
            foreach (var path in _options.Storage.CustomStorePaths)
            {
                stores.Add(new FileMessageStore(path));
            }
        }

        // User-provided stores
        if (_options.CustomStores?.Any() == true)
        {
            stores.AddRange(_options.CustomStores);
        }

        // Apply to registry
        if (stores.Any())
        {
            MessageRegistry.UseStores(stores.ToArray());
        }
    }

    private void ConfigureFormatters()
    {
        if (_options.CustomFormatters?.Any() == true)
        {
            foreach (var (name, factory) in _options.CustomFormatters)
            {
                FormatterRegistry.Register(name, factory);
            }
        }
    }

    private void ConfigureInterceptors()
    {
        // Auto-logging interceptor
        if (_options.Logging.AutoLog)
        {
            var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("EasyMessages");
            InterceptorRegistry.Register(new LoggingInterceptor(() => logger));
        }

        // Correlation ID interceptor
        if (_options.Interceptor.AutoAddCorrelationId)
        {
            var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            InterceptorRegistry.Register(new CorrelationIdInterceptor(() => httpContextAccessor));
        }

        // Metadata enrichment interceptor
        if (_options.Interceptor.AutoEnrichMetadata)
        {
            var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            InterceptorRegistry.Register(
                new MetadataEnrichmentInterceptor(
                    () => httpContextAccessor,
                    _options.Interceptor.MetadataFields
                )
            );
        }

        // User-provided interceptors
        if (_options.Interceptors?.Any() == true)
        {
            foreach (var interceptor in _options.Interceptors)
            {
                InterceptorRegistry.Register(interceptor);
            }
        }
    }
}

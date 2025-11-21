using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore.Interceptors;
using RecurPixel.EasyMessages.Core;
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

        // Configure stores based on options
        ConfigureStores(options);

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

    private static void ConfigureInterceptors(
        IServiceCollection services,
        MessageConfiguration options
    )
    {
        // Auto-logging interceptor
        if (options.AutoLog)
        {
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<MessageRegistry>>();
            InterceptorRegistry.Register(new LoggingInterceptor(logger));
        }

        // User-provided interceptors
        if (options.Interceptors?.Any() == true)
        {
            foreach (var interceptor in options.Interceptors)
            {
                InterceptorRegistry.Register(interceptor);
            }
        }
    }
}

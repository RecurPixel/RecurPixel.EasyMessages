using Microsoft.Extensions.Hosting;

namespace RecurPixel.EasyMessages.AspNetCore.Configuration;

/// <summary>
/// Background service that configures EasyMessages on application startup
/// </summary>
/// <remarks>
/// This hosted service ensures EasyMessages is fully configured before
/// the application starts processing requests. It triggers the configurator
/// which applies all validated options.
/// </remarks>
internal class EasyMessagesStartupService : IHostedService
{
    private readonly EasyMessagesConfigurator _configurator;

    public EasyMessagesStartupService(EasyMessagesConfigurator configurator)
    {
        _configurator = configurator;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Configure EasyMessages on startup
        _configurator.Configure();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // No cleanup needed
        return Task.CompletedTask;
    }
}

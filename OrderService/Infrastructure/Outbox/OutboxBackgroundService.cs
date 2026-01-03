using Microsoft.Extensions.Hosting;

namespace OrderService.Infrastructure.Outbox
{
    /// <summary>
    /// Hosted service that starts the Outbox publisher on application startup.
    /// </summary>
    public sealed class OutboxBackgroundService : IHostedService
    {
        private readonly OutboxPublisherService _publisher;
        private readonly IHostApplicationLifetime _lifetime;

        public OutboxBackgroundService(
            OutboxPublisherService publisher,
            IHostApplicationLifetime lifetime)
        {
            _publisher = publisher;
            _lifetime = lifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start polling outbox when the app starts
            _publisher.StartPublishing(_lifetime.ApplicationStopping);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Graceful shutdown handled inside publisher
            _publisher.Dispose();
            return Task.CompletedTask;
        }
    }
}

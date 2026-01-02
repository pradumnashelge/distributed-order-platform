using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Events;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Outbox
{
    /// <summary>
    /// Background service that reads unprocessed outbox messages and publishes them to messaging systems.
    /// </summary>
    public sealed class OutboxPublisherService : IOutboxPublisher, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MassTransitPublisher _massTransit;
        private readonly KafkaProducer _kafkaProducer;
        private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private CancellationTokenSource? _internalCts;
        private Task? _runningTask;

        public OutboxPublisherService(
            IServiceScopeFactory scopeFactory,
            MassTransitPublisher massTransit,
            KafkaProducer kafkaProducer)
        {
            _scopeFactory = scopeFactory;
            _massTransit = massTransit;
            _kafkaProducer = kafkaProducer;
        }

        /// <inheritdoc />
        public void StartPublishing(CancellationToken applicationStopping)
        {
            if (_runningTask != null) return;

            _internalCts = CancellationTokenSource.CreateLinkedTokenSource(applicationStopping);
            _runningTask = Task.Run(() => RunAsync(_internalCts.Token), CancellationToken.None);
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBatchAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception)
                {
                    // Swallow, but in production add metrics/alerts
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task ProcessBatchAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var batch = await db.OutboxMessages
                .Where(x => !x.Processed)
                .OrderBy(x => x.OccurredOn)
                .Take(50)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var item in batch)
            {
                try
                {
                    if (item.Type == nameof(OrderCreatedEvent))
                    {
                        var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(item.Payload, _serializerOptions)
                                  ?? throw new InvalidOperationException("Failed to deserialize payload.");

                        // Publish to RabbitMQ via MassTransit
                        await _massTransit.PublishOrderCreatedAsync(evt, cancellationToken).ConfigureAwait(false);

                        // Publish to Kafka topic "order-events"
                        await _kafkaProducer.ProduceAsync(evt.OrderId.ToString(), evt, cancellationToken).ConfigureAwait(false);
                    }

                    item.Processed = true;
                    db.OutboxMessages.Update(item);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // If publishing fails, do not mark processed. Move on to next message so we can retry others.
                    // In production, add backoff, poison queue handling, logging.
                }
            }
        }

        public void Dispose()
        {
            _internalCts?.Cancel();
            try
            {
                _runningTask?.Wait(TimeSpan.FromSeconds(5));
            }
            catch { /* ignored */ }

            _internalCts?.Dispose();
            _kafkaProducer.Dispose();
        }
    }
}
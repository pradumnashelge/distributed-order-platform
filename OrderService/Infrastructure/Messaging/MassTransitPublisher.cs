using MassTransit;
using System.Text.Json;
using OrderService.Domain.Events;

namespace OrderService.Infrastructure.Messaging
{
    /// <summary>
    /// Adapter to MassTransit for publishing events to RabbitMQ.
    /// </summary>
    public sealed class MassTransitPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public MassTransitPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        /// <summary>
        /// Publishes an order-created event to RabbitMQ.
        /// </summary>
        public Task PublishOrderCreatedAsync(OrderCreatedEvent @event, CancellationToken cancellationToken)
        {
            // Exchange name routing/exchange configured globally in MassTransit configuration.
            return _publishEndpoint.Publish(@event, ctx =>
            {
                ctx.Durable = true;
            }, cancellationToken);
        }
    }
}
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Events;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Application.Handlers
{
    /// <summary>
    /// Result model for create order handler.
    /// </summary>
    public sealed class CreateOrderResult
    {
        public Guid OrderId { get; init; }
        public string Status { get; init; } = string.Empty;
    }

    /// <summary>
    /// Handles <see cref="OrderService.Application.Commands.CreateOrderCommand"/>.
    /// Persists Order and corresponding OutboxMessage in a single transaction.
    /// </summary>
    public sealed class CreateOrderHandler
    {
        private readonly OrderDbContext _dbContext;
        private readonly JsonSerializerOptions _serializerOptions;

        public CreateOrderHandler(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
            _serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        /// <summary>
        /// Handles create order command.
        /// </summary>
        /// <param name="command">Command data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result containing order id and status.</returns>
        public async Task<CreateOrderResult> HandleAsync(Application.Commands.CreateOrderCommand command, CancellationToken cancellationToken)
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            var order = new Order(command.ProductId, command.Quantity, command.Price);

            var @event = new OrderCreatedEvent(order.Id, order.ProductId, order.Quantity, order.Price, order.CreatedAt);
            var payload = JsonSerializer.Serialize(@event, _serializerOptions);

            var outbox = new Infrastructure.Persistence.OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = nameof(OrderCreatedEvent),
                Payload = payload,
                OccurredOn = DateTime.UtcNow,
                Processed = false
            };

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await _dbContext.Orders.AddAsync(order, cancellationToken).ConfigureAwait(false);
                await _dbContext.OutboxMessages.AddAsync(outbox, cancellationToken).ConfigureAwait(false);

                await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }

            return new CreateOrderResult { OrderId = order.Id, Status = order.Status };
        }
    }
}
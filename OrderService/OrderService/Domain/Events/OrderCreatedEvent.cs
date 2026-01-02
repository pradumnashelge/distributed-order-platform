using System;

namespace OrderService.Domain.Events
{
    /// <summary>
    /// Event published when an order is created.
    /// </summary>
    public sealed record OrderCreatedEvent(Guid OrderId, string ProductId, int Quantity, decimal Price, DateTime CreatedAt);
}
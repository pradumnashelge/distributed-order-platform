namespace AnalyticsService.Consumers;

public record OrderCreatedEvent(
    Guid OrderId,
    string ProductId,
    int Quantity,
    decimal Price,
    DateTime CreatedAt
);

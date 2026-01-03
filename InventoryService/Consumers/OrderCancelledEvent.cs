namespace InventoryService.Consumers;

public record OrderCancelledEvent(
    Guid OrderId,
    string ProductId,
    int Quantity
);

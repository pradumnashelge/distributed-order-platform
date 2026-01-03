using MassTransit;
using Microsoft.EntityFrameworkCore;
using InventoryService.Infrastructure;

namespace InventoryService.Consumers;

public class OrderCancelledConsumer : IConsumer<OrderCancelledEvent>
{
    private readonly InventoryDbContext _dbContext;

    public OrderCancelledConsumer(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var message = context.Message;

        var item = await _dbContext.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == message.ProductId);

        if (item == null) return;

        item.AvailableQuantity += message.Quantity;
        item.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }
}

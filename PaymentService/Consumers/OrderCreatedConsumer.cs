using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Domain;
using PaymentService.Infrastructure;

namespace PaymentService.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly PaymentDbContext _dbContext;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(
        PaymentDbContext dbContext,
        ILogger<OrderCreatedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        // Idempotency check
        var exists = await _dbContext.Payments
            .AnyAsync(p => p.OrderId == message.OrderId);

        if (exists)
        {
            _logger.LogInformation(
                "Payment already processed for Order {OrderId}",
                message.OrderId);
            return;
        }

        // Simulate payment processing
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = message.OrderId,
            Amount = message.Price * message.Quantity,
            Status = "Completed"
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Payment successful for Order {OrderId}",
            message.OrderId);
    }
}

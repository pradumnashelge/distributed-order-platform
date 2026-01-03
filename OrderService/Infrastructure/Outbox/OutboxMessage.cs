namespace OrderService.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public string Type { get; set; } = default!;

    public string Payload { get; set; } = default!;

    public DateTime OccurredOn { get; set; }

    public bool Processed { get; set; }
}

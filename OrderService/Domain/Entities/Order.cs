namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }

    public string ProductId { get; set; } = default!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string Status { get; set; } = "Created";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

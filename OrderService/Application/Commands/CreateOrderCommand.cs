namespace OrderService.Application.Commands;

public class CreateOrderCommand
{
    public string ProductId { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

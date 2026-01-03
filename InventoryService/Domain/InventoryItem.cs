namespace InventoryService.Domain;

public class InventoryItem
{
    public Guid Id { get; set; }
    public string ProductId { get; set; } = default!;
    public int AvailableQuantity { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

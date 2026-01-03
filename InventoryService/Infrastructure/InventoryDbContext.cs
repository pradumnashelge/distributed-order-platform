using Microsoft.EntityFrameworkCore;
using InventoryService.Domain;

namespace InventoryService.Infrastructure;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options) { }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => i.ProductId).IsUnique();
            entity.Property(i => i.ProductId).IsRequired();
        });
    }
}

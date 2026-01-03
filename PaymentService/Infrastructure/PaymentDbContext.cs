using Microsoft.EntityFrameworkCore;
using PaymentService.Domain;

namespace PaymentService.Infrastructure;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options) { }

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.OrderId).IsUnique(); // Idempotency
            entity.Property(p => p.Amount).HasColumnType("numeric(18,2)");
        });
    }
}

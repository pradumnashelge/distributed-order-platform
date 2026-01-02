using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core DbContext for orders and outbox messages.
    /// </summary>
    public sealed class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OutboxMessage>().ToTable("OutboxMessages");
        }
    }
}
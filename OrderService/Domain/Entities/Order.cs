using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Domain.Entities
{
    /// <summary>
    /// Domain entity representing an Order.
    /// </summary>
    public sealed class Order
    {
        /// <summary>
        /// Order identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; private set; }

        /// <summary>
        /// Product identifier.
        /// </summary>
        [Required]
        public string ProductId { get; private set; }

        /// <summary>
        /// Quantity ordered.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Price per unit.
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// Order status.
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Creation time in UTC.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        private Order()
        {
            // For EF
            Id = Guid.Empty;
            ProductId = string.Empty;
            Status = string.Empty;
            CreatedAt = DateTime.MinValue;
        }

        public Order(string productId, int quantity, decimal price)
        {
            Id = Guid.NewGuid();
            ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
            Quantity = quantity;
            Price = price;
            Status = "Created";
            CreatedAt = DateTime.UtcNow;
        }
    }
}
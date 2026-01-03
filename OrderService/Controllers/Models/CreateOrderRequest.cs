using System.ComponentModel.DataAnnotations;

namespace OrderService.Controllers.Models
{
    /// <summary>
    /// Request DTO for creating orders.
    /// </summary>
    public sealed class CreateOrderRequest
    {
        /// <summary>
        /// Product identifier.
        /// </summary>
        [Required]
        public string ProductId { get; init; } = string.Empty;

        /// <summary>
        /// Quantity to order.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Quantity { get; init; }

        /// <summary>
        /// Price per unit.
        /// </summary>
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; init; }
    }
}
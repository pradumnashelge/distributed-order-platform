using System;

namespace OrderService.Application.Commands
{
    /// <summary>
    /// Command to create a new order.
    /// </summary>
    public sealed class CreateOrderCommand
    {
        /// <summary>
        /// Product identifier.
        /// </summary>
        public string ProductId { get; }

        /// <summary>
        /// Quantity to order.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Price per unit.
        /// </summary>
        public decimal Price { get; }

        public CreateOrderCommand(string productId, int quantity, decimal price)
        {
            ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
            Quantity = quantity;
            Price = price;
        }
    }
}
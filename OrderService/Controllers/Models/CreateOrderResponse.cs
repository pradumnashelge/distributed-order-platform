namespace OrderService.Controllers.Models
{
    /// <summary>
    /// Response DTO for created order.
    /// </summary>
    public sealed class CreateOrderResponse
    {
        /// <summary>
        /// The created order id.
        /// </summary>
        public Guid OrderId { get; }

        /// <summary>
        /// The order status.
        /// </summary>
        public string Status { get; }

        public CreateOrderResponse(Guid orderId, string status)
        {
            OrderId = orderId;
            Status = status;
        }
    }
}
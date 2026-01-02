using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.Handlers;
using OrderService.Controllers.Models;

namespace OrderService.Controllers
{
    /// <summary>
    /// API controller for managing orders.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly CreateOrderHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersController"/> class.
        /// </summary>
        /// <param name="handler">Command handler for creating orders.</param>
        public OrdersController(CreateOrderHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="request">Order creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Created order id and status.</returns>
        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> CreateOrderAsync(
            [FromBody, Required] CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CreateOrderCommand(request.ProductId, request.Quantity, request.Price);
            var result = await _handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return Accepted(new CreateOrderResponse(result.OrderId, result.Status));
        }
    }
}
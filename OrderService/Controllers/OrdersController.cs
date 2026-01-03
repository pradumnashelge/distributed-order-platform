using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.Handlers;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderHandler _handler;

    public OrdersController(CreateOrderHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var orderId = await _handler.HandleAsync(command);

        return Ok(new
        {
            OrderId = orderId,
            Status = "Created"
        });
    }
}

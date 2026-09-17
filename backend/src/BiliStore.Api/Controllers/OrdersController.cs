using System.Security.Claims;
using BiliStore.Application.DTOs.Orders;
using BiliStore.Application.Orders;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateOrderResponseDto>> CreateOrder(
        CreateOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var applicationUserId = GetApplicationUserId();
        var response = await orderService.CreateAsync(request, applicationUserId, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{orderNumber}")]
    public async Task<ActionResult<OrderConfirmationDto>> GetOrder(
        string orderNumber,
        CancellationToken cancellationToken)
    {
        var order = await orderService.GetByOrderNumberAsync(orderNumber, cancellationToken);

        return order is null ? NotFound() : Ok(order);
    }

    private Guid? GetApplicationUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}

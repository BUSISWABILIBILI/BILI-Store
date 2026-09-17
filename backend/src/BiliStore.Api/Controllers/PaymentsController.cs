using BiliStore.Application.DTOs.Payments;
using BiliStore.Application.Payments;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(IPaymentSessionService paymentSessionService) : ControllerBase
{
    [HttpPost("create-session")]
    public async Task<ActionResult<CreatePaymentSessionResponseDto>> CreateSession(
        CreatePaymentSessionRequestDto request,
        CancellationToken cancellationToken)
    {
        var session = await paymentSessionService.CreateSessionAsync(request, cancellationToken);

        return Ok(session);
    }
}

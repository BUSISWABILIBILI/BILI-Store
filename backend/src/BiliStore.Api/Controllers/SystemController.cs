using BiliStore.Application.DTOs.System;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiInfoResponse> Get()
    {
        return Ok(new ApiInfoResponse(
            Name: "BILI Store",
            Version: "1.4.0",
            Description: "Delivery operations, customer returns, admin dashboard, admin customer, admin catalogue, admin order, payment, checkout, customer account, catalogue, measurement, and cart API for a custom-sewn traditional clothing ordering system.",
            CurrentPhase: "Phase 14 - Delivery and return operations"));
    }
}

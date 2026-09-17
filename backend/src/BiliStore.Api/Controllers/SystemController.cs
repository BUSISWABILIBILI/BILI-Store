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
            Version: "0.8.0",
            Description: "Checkout, customer account, catalogue, measurement, and cart API for a custom-sewn traditional clothing ordering system.",
            CurrentPhase: "Phase 8 - Checkout and PAXI/PEP delivery"));
    }
}

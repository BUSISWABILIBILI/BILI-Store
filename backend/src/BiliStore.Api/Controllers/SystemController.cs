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
            Version: "1.1.0",
            Description: "Admin, order, payment, checkout, customer account, catalogue, measurement, and cart API for a custom-sewn traditional clothing ordering system.",
            CurrentPhase: "Phase 11 - Admin order management"));
    }
}

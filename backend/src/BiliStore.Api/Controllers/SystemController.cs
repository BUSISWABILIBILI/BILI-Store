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
            Version: "0.2.0",
            Description: "Foundation API for a custom-sewn traditional clothing ordering system.",
            CurrentPhase: "Phase 2 - Project setup"));
    }
}


using BiliStore.Application.DTOs.System;
using Microsoft.AspNetCore.Mvc;

namespace BiliStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController(IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public ActionResult<HealthCheckResponse> Get()
    {
        return Ok(new HealthCheckResponse(
            Service: "BILI Store API",
            Status: "Healthy",
            Environment: environment.EnvironmentName,
            ServerTime: DateTimeOffset.UtcNow));
    }
}


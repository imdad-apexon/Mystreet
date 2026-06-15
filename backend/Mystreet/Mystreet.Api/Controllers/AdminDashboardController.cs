using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystreet.Application.Interfaces;

namespace Mystreet.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/admin/dashboard")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _service;

    public AdminDashboardController(IAdminDashboardService service)
    {
        _service = service;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview([FromQuery] int trendDays = 14)
    {
        if (!User.HasClaim("isAdmin", "true")) return Forbid();

        return Ok(await _service.GetOverviewAsync(trendDays));
    }
}
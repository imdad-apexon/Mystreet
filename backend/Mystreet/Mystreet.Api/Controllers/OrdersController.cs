using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystreet.Application.DTOs.Orders;
using Mystreet.Application.Interfaces;
using System.Security.Claims;

namespace Mystreet.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    public OrdersController(IOrderService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var orderId = await _service.CreateAsync(userId, dto);
        return Ok(new { orderId });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetMineAsync(userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.HasClaim("isAdmin", "true");
        var result = await _service.GetByIdAsync(userId, id, isAdmin);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.HasClaim("isAdmin", "true");
        return await _service.CancelAsync(userId, id, isAdmin) ? Ok() : NotFound();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystreet.Application.DTOs.Products;
using Mystreet.Application.Interfaces;
using System.Security.Claims;

namespace Mystreet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? brand, [FromQuery] string? size, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        => Ok(await _service.GetAllAsync(brand, size, minPrice, maxPrice));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _service.GetByIdAsync(id));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (!User.HasClaim("isAdmin", "true")) return Forbid();
        return Ok(await _service.CreateAsync(dto));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CreateProductDto dto)
    {
        if (!User.HasClaim("isAdmin", "true")) return Forbid();
        var result = await _service.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!User.HasClaim("isAdmin", "true")) return Forbid();
        return await _service.DeleteAsync(id) ? Ok() : NotFound();
    }
}
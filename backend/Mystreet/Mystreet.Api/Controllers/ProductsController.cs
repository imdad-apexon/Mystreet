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
    public async Task<IActionResult> GetAll([FromQuery] string? brand, [FromQuery] string? size, [FromQuery] string? category, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        => Ok(await _service.GetAllAsync(brand, size, category, minPrice, maxPrice));

    [HttpGet("ai-search")]
    public async Task<IActionResult> SearchAi([FromQuery] string query, [FromQuery] string? model, [FromQuery] int? limit)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("query is required.");

        return Ok(await _service.SearchNaturalLanguageAsync(query, model, limit));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _service.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

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
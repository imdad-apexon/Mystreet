using Mystreet.Application.DTOs.Products;

namespace Mystreet.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync(string? brand, string? size, string? category, decimal? minPrice, decimal? maxPrice);
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateAsync(Guid id, CreateProductDto dto);
    Task<bool> DeleteAsync(Guid id);
}
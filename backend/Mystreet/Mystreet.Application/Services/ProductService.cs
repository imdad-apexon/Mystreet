using Microsoft.EntityFrameworkCore;
using Mystreet.Application.DTOs.Products;
using Mystreet.Application.Interfaces;
using Mystreet.Domain.Entities;
using Mystreet.Infrastructure.Data;

namespace Mystreet.Application.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ProductDto>> GetAllAsync(string? brand, string? size, decimal? minPrice, decimal? maxPrice)
    {
        var query = _db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(x => x.Brand == brand);

        if (!string.IsNullOrWhiteSpace(size))
            query = query.Where(x => x.SizesCsv.Contains(size));

        if (minPrice.HasValue)
            query = query.Where(x => x.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(x => x.Price <= maxPrice.Value);

        return await query
            .Where(x => x.IsActive)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Brand = x.Brand,
                Description = x.Description,
                Price = x.Price,
                SizesCsv = x.SizesCsv,
                StockQty = x.StockQty,
                ImageUrl = x.ImageUrl,
                Category = x.Category
            })
            .ToListAsync();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var x = await _db.Products.FindAsync(id);
        return x is null ? null : new ProductDto
        {
            Id = x.Id,
            Name = x.Name,
            Brand = x.Brand,
            Description = x.Description,
            Price = x.Price,
            SizesCsv = x.SizesCsv,
            StockQty = x.StockQty,
            ImageUrl = x.ImageUrl,
            Category = x.Category
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Brand = dto.Brand,
            Description = dto.Description,
            Price = dto.Price,
            SizesCsv = dto.SizesCsv,
            StockQty = dto.StockQty,
            ImageUrl = dto.ImageUrl,
            Category = dto.Category
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return await GetByIdAsync(product.Id) ?? throw new Exception("Create failed.");
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, CreateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return null;

        product.Name = dto.Name;
        product.Brand = dto.Brand;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.SizesCsv = dto.SizesCsv;
        product.StockQty = dto.StockQty;
        product.ImageUrl = dto.ImageUrl;
        product.Category = dto.Category;

        await _db.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }
}
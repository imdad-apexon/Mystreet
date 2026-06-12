using Microsoft.EntityFrameworkCore;
using Mystreet.Application.DTOs.Products;
using Mystreet.Application.Interfaces;
using Mystreet.Domain.Entities;
using Mystreet.Infrastructure.Data;

namespace Mystreet.Application.Services;

public class ProductService : IProductService
{
    private const decimal MinPrice = 0.01m;
    private const decimal MaxPrice = 1000000m;
    private const int MaxStockQty = 10000;
    private const int MinNameLength = 3;
    private const int MaxNameLength = 200;
    private const string DuplicateProductMessage = "A product with the same name, brand, category, price, and sizes already exists.";
    private const int DefaultNaturalLanguageResultLimit = 30;
    private const int MaxNaturalLanguageResultLimit = 100;

    private readonly AppDbContext _db;
    private readonly IProductQueryUnderstandingService _queryUnderstandingService;

    public ProductService(AppDbContext db, IProductQueryUnderstandingService queryUnderstandingService)
    {
        _db = db;
        _queryUnderstandingService = queryUnderstandingService;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(string? brand, string? size, string? category, decimal? minPrice, decimal? maxPrice)
    {
        var query = _db.Products.AsQueryable();

        var normalizedBrand = brand?.Trim().ToLower();
        var normalizedCategory = category?.Trim().ToLower();
        var normalizedSize = size?.Trim().ToLower();

        if (!string.IsNullOrWhiteSpace(normalizedBrand))
            query = query.Where(x => x.Brand.ToLower() == normalizedBrand);

        if (!string.IsNullOrWhiteSpace(normalizedCategory))
            query = query.Where(x => x.Category.ToLower() == normalizedCategory);

        if (!string.IsNullOrWhiteSpace(normalizedSize))
            query = query.Where(x => x.SizesCsv.ToLower().Contains(normalizedSize));

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

    public async Task<IEnumerable<ProductDto>> SearchNaturalLanguageAsync(string query, string? model = null, int? limit = null)
    {
        var cleanQuery = query?.Trim() ?? string.Empty;
        if (cleanQuery.Length == 0) return [];

        var parsed = await _queryUnderstandingService.ParseAsync(cleanQuery, model);
        var queryable = _db.Products.Where(x => x.IsActive).AsQueryable();

        if (!string.IsNullOrWhiteSpace(parsed.Brand))
            queryable = queryable.Where(x => x.Brand.ToLower().Contains(parsed.Brand));

        if (!string.IsNullOrWhiteSpace(parsed.Category))
            queryable = queryable.Where(x => x.Category.ToLower().Contains(parsed.Category));

        if (!string.IsNullOrWhiteSpace(parsed.Size))
            queryable = queryable.Where(x => x.SizesCsv.ToLower().Contains(parsed.Size));

        if (parsed.MinPrice.HasValue)
            queryable = queryable.Where(x => x.Price >= parsed.MinPrice.Value);

        if (parsed.MaxPrice.HasValue)
            queryable = queryable.Where(x => x.Price <= parsed.MaxPrice.Value);

        var candidates = await queryable
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

        var effectiveLimit = Math.Clamp(limit ?? DefaultNaturalLanguageResultLimit, 1, MaxNaturalLanguageResultLimit);
        if (parsed.Keywords.Count == 0)
            return candidates
                .OrderBy(x => x.Price)
                .Take(effectiveLimit)
                .ToList();

        var ranked = candidates
            .Select(p => new { Product = p, Score = ScoreProduct(p, parsed.Keywords) })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Product.Price)
            .Select(x => x.Product)
            .Take(effectiveLimit)
            .ToList();

        if (ranked.Count > 0)
            return ranked;

        return candidates
            .OrderBy(x => x.Price)
            .Take(effectiveLimit)
            .ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var x = await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
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
        ValidateAndNormalize(dto);
        await EnsureNoActiveDuplicateAsync(dto);

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

        ValidateAndNormalize(dto);
        await EnsureNoActiveDuplicateAsync(dto, id);

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

    private static void ValidateAndNormalize(CreateProductDto dto)
    {
        dto.Name = dto.Name?.Trim() ?? string.Empty;
        dto.Brand = dto.Brand?.Trim() ?? string.Empty;
        dto.Description = dto.Description?.Trim() ?? string.Empty;
        dto.SizesCsv = dto.SizesCsv?.Trim() ?? string.Empty;
        dto.ImageUrl = dto.ImageUrl?.Trim() ?? string.Empty;
        dto.Category = dto.Category?.Trim() ?? string.Empty;

        if (dto.Name.Length < MinNameLength || dto.Name.Length > MaxNameLength)
            throw new InvalidOperationException($"Product name must be between {MinNameLength} and {MaxNameLength} characters.");

        if (dto.Price < MinPrice || dto.Price > MaxPrice)
            throw new InvalidOperationException($"Price must be between {MinPrice:0.00} and {MaxPrice:0.##}.");

        if (dto.StockQty < 0 || dto.StockQty > MaxStockQty)
            throw new InvalidOperationException($"Stock quantity must be between 0 and {MaxStockQty}.");
    }

    private async Task EnsureNoActiveDuplicateAsync(CreateProductDto dto, Guid? excludingProductId = null)
    {
        var duplicateQuery = _db.Products.Where(p =>
            p.IsActive
            && p.Name.ToLower() == dto.Name.ToLower()
            && p.Brand.ToLower() == dto.Brand.ToLower()
            && p.Category.ToLower() == dto.Category.ToLower()
            && p.SizesCsv.ToLower() == dto.SizesCsv.ToLower()
            && p.Price == dto.Price);

        if (excludingProductId.HasValue)
            duplicateQuery = duplicateQuery.Where(p => p.Id != excludingProductId.Value);

        if (await duplicateQuery.AnyAsync())
            throw new InvalidOperationException(DuplicateProductMessage);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;

        product.IsActive = false;
        await _db.SaveChangesAsync();
        return true;
    }

    private static int ScoreProduct(ProductDto product, IEnumerable<string> keywords)
    {
        var name = product.Name.ToLowerInvariant();
        var brand = product.Brand.ToLowerInvariant();
        var category = product.Category.ToLowerInvariant();
        var description = product.Description.ToLowerInvariant();

        var score = 0;
        foreach (var keyword in keywords)
        {
            if (name.Contains(keyword)) score += 5;
            if (brand.Contains(keyword)) score += 4;
            if (category.Contains(keyword)) score += 3;
            if (description.Contains(keyword)) score += 2;
        }

        return score;
    }
}
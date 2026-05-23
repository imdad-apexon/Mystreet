using Mystreet.Tests.Fixtures;

namespace Mystreet.Tests.Services;

public class ProductServiceTests
{
    private readonly TestDbFixture _fixture = new();

    [Fact]
    public async Task GetAllAsync_ShouldReturnFilteredProducts()
    {
        await using var db = _fixture.CreateDbContext();
        db.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Name = "Air Max 90", Brand = "Nike", Price = 120, SizesCsv = "7,8,9", StockQty = 10, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true },
            new Product { Id = Guid.NewGuid(), Name = "Ultraboost", Brand = "Adidas", Price = 140, SizesCsv = "9,10,11", StockQty = 12, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true }
        );
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = await service.GetAllAsync("Nike", "8", null, null);

        result.Should().HaveCount(1);
        result.First().Brand.Should().Be("Nike");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
    {
        await using var db = _fixture.CreateDbContext();
        var id = Guid.NewGuid();
        db.Products.Add(new Product
        {
            Id = id,
            Name = "Air Max 90",
            Brand = "Nike",
            Price = 120,
            SizesCsv = "7,8,9",
            StockQty = 10,
            ImageUrl = "",
            Description = "",
            Category = "Sneakers",
            IsActive = true
        });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        var result = await service.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistProduct()
    {
        await using var db = _fixture.CreateDbContext();
        var service = new ProductService(db);

        var dto = new CreateProductDto
        {
            Name = "New Shoe",
            Brand = "Puma",
            Description = "Test",
            Price = 99,
            SizesCsv = "8,9",
            StockQty = 5,
            ImageUrl = "img",
            Category = "Sneakers"
        };

        var result = await service.CreateAsync(dto);

        result.Name.Should().Be("New Shoe");
        (await db.Products.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenMissing()
    {
        await using var db = _fixture.CreateDbContext();
        var service = new ProductService(db);

        var result = await service.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }
}
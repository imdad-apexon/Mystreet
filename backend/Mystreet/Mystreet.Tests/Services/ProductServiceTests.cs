using Mystreet.Tests.Fixtures;

namespace Mystreet.Tests.Services;

public class ProductServiceTests
{
    private readonly TestDbFixture _fixture = new();

    #region GetAll Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts_WhenNoFiltersApplied()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var product1 = new Product 
        { 
            Id = Guid.NewGuid(), 
            Name = "Air Max 90", 
            Brand = "Nike", 
            Price = 120, 
            SizesCsv = "7,8,9", 
            StockQty = 10, 
            ImageUrl = "air-max.png", 
            Description = "Classic sneaker", 
            Category = "Sneakers", 
            IsActive = true 
        };
        var product2 = new Product 
        { 
            Id = Guid.NewGuid(), 
            Name = "Ultraboost", 
            Brand = "Adidas", 
            Price = 140, 
            SizesCsv = "9,10,11", 
            StockQty = 12, 
            ImageUrl = "ultra.png", 
            Description = "Comfort shoe", 
            Category = "Sneakers", 
            IsActive = true 
        };
        db.Products.AddRange(product1, product2);
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        // Act
        var result = await service.GetAllAsync(null, null, null, null, null);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Brand == "Nike");
        result.Should().Contain(p => p.Brand == "Adidas");
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByBrand()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        db.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Name = "Air Max 90", Brand = "Nike", Price = 120, SizesCsv = "7,8,9", StockQty = 10, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true },
            new Product { Id = Guid.NewGuid(), Name = "Ultraboost", Brand = "Adidas", Price = 140, SizesCsv = "9,10,11", StockQty = 12, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true }
        );
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        // Act
        var result = await service.GetAllAsync("Nike", null, null, null, null);

        // Assert
        result.Should().HaveCount(1);
        result.First().Brand.Should().Be("Nike");
        result.First().Name.Should().Be("Air Max 90");
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByBrand_CaseInsensitive()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        db.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Name = "Air Max 90", Brand = "Nike", Price = 120, SizesCsv = "7,8,9", StockQty = 10, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true },
            new Product { Id = Guid.NewGuid(), Name = "Ultraboost", Brand = "Adidas", Price = 140, SizesCsv = "9,10,11", StockQty = 12, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true }
        );
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        // Act
        var result = await service.GetAllAsync("niKE", null, null, null, null);

        // Assert
        result.Should().HaveCount(1);
        result.First().Brand.Should().Be("Nike");
        result.First().Name.Should().Be("Air Max 90");
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByCategory()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        db.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Name = "Air Max 90", Brand = "Nike", Price = 120, SizesCsv = "7,8,9", StockQty = 10, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true },
            new Product { Id = Guid.NewGuid(), Name = "Work Boot", Brand = "Timberland", Price = 150, SizesCsv = "9,10,11", StockQty = 8, ImageUrl = "", Description = "", Category = "Boots", IsActive = true }
        );
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        // Act
        var result = await service.GetAllAsync(null, null, "Sneakers", null, null);

        // Assert
        result.Should().HaveCount(1);
        result.First().Category.Should().Be("Sneakers");
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterBySize()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        db.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Name = "Air Max 90", Brand = "Nike", Price = 120, SizesCsv = "7,8,9", StockQty = 10, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true },
            new Product { Id = Guid.NewGuid(), Name = "Ultraboost", Brand = "Adidas", Price = 140, SizesCsv = "9,10,11", StockQty = 12, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true }
        );
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        // Act
        var result = await service.GetAllAsync(null, "10", null, null, null);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Ultraboost");
        result.First().SizesCsv.Should().Contain("10");
    }

    [Fact]
    public async Task GetAllAsync_ShouldApplyMultipleFilters()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        db.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Name = "Air Max 90", Brand = "Nike", Price = 120, SizesCsv = "7,8,9", StockQty = 10, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true },
            new Product { Id = Guid.NewGuid(), Name = "Nike Runner", Brand = "Nike", Price = 100, SizesCsv = "9,10,11", StockQty = 15, ImageUrl = "", Description = "", Category = "Running", IsActive = true },
            new Product { Id = Guid.NewGuid(), Name = "Ultraboost", Brand = "Adidas", Price = 140, SizesCsv = "9,10,11", StockQty = 12, ImageUrl = "", Description = "", Category = "Sneakers", IsActive = true }
        );
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        // Act
        var result = await service.GetAllAsync("Nike", "9", "Sneakers", null, null);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Air Max 90");
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
    {
        // Arrange
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
            ImageUrl = "air-max.png",
            Description = "Classic",
            Category = "Sneakers",
            IsActive = true
        });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        // Act
        var result = await service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Air Max 90");
        result.Brand.Should().Be("Nike");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var service = new ProductService(db);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await service.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductIsInactive()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var id = Guid.NewGuid();
        db.Products.Add(new Product
        {
            Id = id,
            Name = "Inactive Product",
            Brand = "Brand",
            Price = 100,
            SizesCsv = "7,8",
            StockQty = 5,
            ImageUrl = "",
            Description = "",
            Category = "Sneakers",
            IsActive = false
        });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        // Act
        var result = await service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldPersistProduct()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var service = new ProductService(db);

        var dto = new CreateProductDto
        {
            Name = "New Shoe",
            Brand = "Puma",
            Description = "High performance shoe",
            Price = 99.99m,
            SizesCsv = "8,9,10",
            StockQty = 25,
            ImageUrl = "puma-new.png",
            Category = "Sneakers"
        };

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Shoe");
        result.Brand.Should().Be("Puma");
        result.Price.Should().Be(99.99m);
        (await db.Products.CountAsync()).Should().Be(1);
        
        var savedProduct = await db.Products.FirstAsync();
        savedProduct.SizesCsv.Should().Be("8,9,10");
        savedProduct.StockQty.Should().Be(25);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetIsActiveToTrue()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var service = new ProductService(db);

        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Brand = "TestBrand",
            Description = "Test",
            Price = 50,
            SizesCsv = "7,8",
            StockQty = 5,
            ImageUrl = "test.png",
            Category = "Test"
        };

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
        result.Brand.Should().Be("TestBrand");
        (await db.Products.CountAsync()).Should().Be(1);
        
        var savedProduct = await db.Products.FirstAsync();
        savedProduct.IsActive.Should().BeTrue();
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task DeleteAsync_WithExistingProduct_ShouldReturnTrue()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var productId = Guid.NewGuid();
        db.Products.Add(new Product
        {
            Id = productId,
            Name = "To Delete",
            Brand = "Brand",
            Price = 100,
            SizesCsv = "8,9",
            StockQty = 5,
            ImageUrl = "",
            Description = "",
            Category = "Test",
            IsActive = true
        });
        await db.SaveChangesAsync();

        var service = new ProductService(db);

        // Act
        var result = await service.DeleteAsync(productId);

        // Assert
        result.Should().BeTrue();
        (await db.Products.CountAsync()).Should().Be(1);
        (await db.Products.FirstAsync(x => x.Id == productId)).IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentProduct_ShouldReturnFalse()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var service = new ProductService(db);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await service.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var productId = Guid.NewGuid();
        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Original Name",
            Brand = "Original Brand",
            Price = 100,
            SizesCsv = "8,9",
            StockQty = 10,
            ImageUrl = "original.png",
            Description = "Original",
            Category = "Original",
            IsActive = true
        });
        await db.SaveChangesAsync();

        var service = new ProductService(db);
        var updateDto = new CreateProductDto
        {
            Name = "Updated Name",
            Brand = "Updated Brand",
            Description = "Updated description",
            Price = 150,
            SizesCsv = "7,8,9,10",
            StockQty = 20,
            ImageUrl = "updated.png",
            Category = "Updated"
        };

        // Act
        var result = await service.UpdateAsync(productId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Brand.Should().Be("Updated Brand");
        result.Price.Should().Be(150);
    }

    #endregion
}
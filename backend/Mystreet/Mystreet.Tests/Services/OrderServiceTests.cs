using Mystreet.Tests.Fixtures;

namespace Mystreet.Tests.Services;

public class OrderServiceTests
{
    private readonly TestDbFixture _fixture = new();

    [Fact]
    public async Task CreateAsync_ShouldCreateOrder_AndReduceStock()
    {
        await using var db = _fixture.CreateDbContext();

        var productId = Guid.NewGuid();
        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Air Max 90",
            Brand = "Nike",
            Price = 120,
            SizesCsv = "8,9,10",
            StockQty = 10,
            ImageUrl = "",
            Description = "",
            Category = "Sneakers",
            IsActive = true
        });
        await db.SaveChangesAsync();

        var service = new OrderService(db);

        var orderId = await service.CreateAsync(Guid.NewGuid(), new CreateOrderDto
        {
            ShippingAddress = "Mumbai",
            PaymentMethod = "COD",
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = productId, Quantity = 2, Size = "9" }
            }
        });

        orderId.Should().NotBeEmpty();
        (await db.Orders.CountAsync()).Should().Be(1);
        (await db.OrderItems.CountAsync()).Should().Be(1);
        (await db.Products.FirstAsync(x => x.Id == productId)).StockQty.Should().Be(8);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenStockInsufficient()
    {
        await using var db = _fixture.CreateDbContext();
        var productId = Guid.NewGuid();
        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Air Max 90",
            Brand = "Nike",
            Price = 120,
            SizesCsv = "8,9,10",
            StockQty = 1,
            ImageUrl = "",
            Description = "",
            Category = "Sneakers",
            IsActive = true
        });
        await db.SaveChangesAsync();

        var service = new OrderService(db);

        var act = async () => await service.CreateAsync(Guid.NewGuid(), new CreateOrderDto
        {
            ShippingAddress = "Mumbai",
            PaymentMethod = "COD",
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = productId, Quantity = 3, Size = "9" }
            }
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Insufficient stock for Air Max 90.");
    }

    [Fact]
    public async Task CancelAsync_ShouldCancelOrder()
    {
        await using var db = _fixture.CreateDbContext();
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();

        db.Orders.Add(new Order
        {
            Id = orderId,
            UserId = userId,
            Status = OrderStatus.Pending,
            ShippingAddress = "Mumbai",
            PaymentMethod = "COD",
            TotalAmount = 100
        });
        await db.SaveChangesAsync();

        var service = new OrderService(db);

        var result = await service.CancelAsync(userId, orderId, false);

        result.Should().BeTrue();
        (await db.Orders.FirstAsync(x => x.Id == orderId)).Status.Should().Be(OrderStatus.Cancelled);
    }
}
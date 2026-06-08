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
    public async Task CreateAsync_ShouldThrow_WhenProductNotFound()
    {
        await using var db = _fixture.CreateDbContext();
        var service = new OrderService(db);
        var act = async () => await service.CreateAsync(Guid.NewGuid(), new CreateOrderDto
        {
            ShippingAddress = "Mumbai",
            PaymentMethod = "COD",
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 1, Size = "9" }
            }
        });
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("One or more products not found.");
    }

    [Fact]
    public async Task GetMineAsync_ShouldReturnOrdersForUser()
    {
        await using var db = _fixture.CreateDbContext();
        var userId = Guid.NewGuid();
        db.Orders.Add(new Order { Id = Guid.NewGuid(), UserId = userId, ShippingAddress = "A", PaymentMethod = "B" });
        db.Orders.Add(new Order { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), ShippingAddress = "C", PaymentMethod = "D" });
        await db.SaveChangesAsync();
        var service = new OrderService(db);
        var result = await service.GetMineAsync(userId);
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders()
    {
        await using var db = _fixture.CreateDbContext();
        var user1 = new User { Id = Guid.NewGuid(), Email = "user1@example.com" };
        var user2 = new User { Id = Guid.NewGuid(), Email = "user2@example.com" };
        db.Users.Add(user1);
        db.Users.Add(user2);
        db.Orders.Add(new Order { Id = Guid.NewGuid(), UserId = user1.Id, User = user1, ShippingAddress = "A", PaymentMethod = "B" });
        db.Orders.Add(new Order { Id = Guid.NewGuid(), UserId = user2.Id, User = user2, ShippingAddress = "C", PaymentMethod = "D" });
        await db.SaveChangesAsync();
        var service = new OrderService(db);
        var result = await service.GetAllAsync();
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenUserOwnsOrder()
    {
        await using var db = _fixture.CreateDbContext();
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        db.Orders.Add(new Order { Id = orderId, UserId = userId, ShippingAddress = "A", PaymentMethod = "B" });
        await db.SaveChangesAsync();
        var service = new OrderService(db);
        var result = await service.GetByIdAsync(userId, orderId, false);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var db = _fixture.CreateDbContext();
        var service = new OrderService(db);
        var result = await service.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid(), false);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenAdmin()
    {
        await using var db = _fixture.CreateDbContext();
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        db.Orders.Add(new Order { Id = orderId, UserId = userId, ShippingAddress = "A", PaymentMethod = "B" });
        await db.SaveChangesAsync();
        var service = new OrderService(db);
        var result = await service.GetByIdAsync(Guid.NewGuid(), orderId, true);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelAsync_ShouldCancelOrder()
    {
        await using var db = _fixture.CreateDbContext();
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Air Max 90",
            Brand = "Nike",
            Price = 120,
            SizesCsv = "8,9,10",
            StockQty = 8,
            ImageUrl = "",
            Description = "",
            Category = "Sneakers",
            IsActive = true
        });

        db.Orders.Add(new Order
        {
            Id = orderId,
            UserId = userId,
            Status = OrderStatus.Pending,
            ShippingAddress = "Mumbai",
            PaymentMethod = "COD",
            TotalAmount = 100,
            Items = new List<OrderItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    ProductName = "Air Max 90",
                    Quantity = 2,
                    Size = "9",
                    UnitPrice = 120
                }
            }
        });
        await db.SaveChangesAsync();
        var service = new OrderService(db);
        var result = await service.CancelAsync(userId, orderId, false);
        result.Should().BeTrue();
        (await db.Orders.FirstAsync(x => x.Id == orderId)).Status.Should().Be(OrderStatus.Cancelled);
        (await db.Products.FirstAsync(x => x.Id == productId)).StockQty.Should().Be(10);
    }

    [Fact]
    public async Task CancelAsync_ShouldReturnFalse_WhenOrderNotFound()
    {
        await using var db = _fixture.CreateDbContext();
        var service = new OrderService(db);
        var result = await service.CancelAsync(Guid.NewGuid(), Guid.NewGuid(), false);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelAsync_ShouldThrow_WhenOrderDelivered()
    {
        await using var db = _fixture.CreateDbContext();
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        db.Orders.Add(new Order
        {
            Id = orderId,
            UserId = userId,
            Status = OrderStatus.Delivered,
            ShippingAddress = "Mumbai",
            PaymentMethod = "COD",
            TotalAmount = 100
        });
        await db.SaveChangesAsync();
        var service = new OrderService(db);
        var act = async () => await service.CancelAsync(userId, orderId, false);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Delivered orders cannot be cancelled.");
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenOrderExists()
    {
        await using var db = _fixture.CreateDbContext();
        var orderId = Guid.NewGuid();
        db.Orders.Add(new Order { Id = orderId, UserId = Guid.NewGuid(), Status = OrderStatus.Pending });
        await db.SaveChangesAsync();
        var service = new OrderService(db);
        var result = await service.UpdateStatusAsync(orderId, OrderStatus.Shipped);
        result.Should().BeTrue();
        (await db.Orders.FirstAsync(x => x.Id == orderId)).Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldRestoreStock_WhenStatusChangesToCancelled()
    {
        await using var db = _fixture.CreateDbContext();
        var productId = Guid.NewGuid();
        var orderId = Guid.NewGuid();

        db.Products.Add(new Product
        {
            Id = productId,
            Name = "Ultraboost",
            Brand = "Adidas",
            Price = 140,
            SizesCsv = "8,9,10",
            StockQty = 6,
            ImageUrl = "",
            Description = "",
            Category = "Sneakers",
            IsActive = true
        });

        db.Orders.Add(new Order
        {
            Id = orderId,
            UserId = Guid.NewGuid(),
            Status = OrderStatus.Pending,
            ShippingAddress = "Mumbai",
            PaymentMethod = "COD",
            TotalAmount = 280,
            Items = new List<OrderItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    ProductName = "Ultraboost",
                    Quantity = 2,
                    Size = "9",
                    UnitPrice = 140
                }
            }
        });

        await db.SaveChangesAsync();
        var service = new OrderService(db);

        var result = await service.UpdateStatusAsync(orderId, OrderStatus.Cancelled);

        result.Should().BeTrue();
        (await db.Orders.FirstAsync(x => x.Id == orderId)).Status.Should().Be(OrderStatus.Cancelled);
        (await db.Products.FirstAsync(x => x.Id == productId)).StockQty.Should().Be(8);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldBeVisibleInCustomerOrderHistory()
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
        await service.UpdateStatusAsync(orderId, OrderStatus.Shipped);

        var history = (await service.GetMineAsync(userId)).ToList();
        history.Should().HaveCount(1);

        var status = history[0].GetType().GetProperty("Status")?.GetValue(history[0]);
        status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnFalse_WhenOrderNotFound()
    {
        await using var db = _fixture.CreateDbContext();
        var service = new OrderService(db);
        var result = await service.UpdateStatusAsync(Guid.NewGuid(), OrderStatus.Shipped);
        result.Should().BeFalse();
    }
}
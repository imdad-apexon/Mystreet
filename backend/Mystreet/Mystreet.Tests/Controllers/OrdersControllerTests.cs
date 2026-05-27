
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Mystreet.Api.Controllers;

namespace Mystreet.Tests.Controllers;

public class OrdersControllerTests
{
    private OrdersController CreateController(Mock<IOrderService> service, ClaimsPrincipal? user = null)
    {
        var controller = new Mystreet.Api.Controllers.OrdersController(service.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() }
        };
        return controller;
    }

    private ClaimsPrincipal CreateUser(Guid userId, bool isAdmin = false)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        if (isAdmin)
            claims.Add(new Claim("isAdmin", "true"));
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WithOrderId()
    {
        var userId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.CreateAsync(userId, It.IsAny<CreateOrderDto>())).ReturnsAsync(Guid.NewGuid());
        var controller = CreateController(service, CreateUser(userId));
        var dto = new CreateOrderDto();
        var result = await controller.Create(dto);
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Mine_ShouldReturnOk_WhenAuthorized()
    {
        var userId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.GetMineAsync(userId)).ReturnsAsync(new List<object>());
        var controller = CreateController(service, CreateUser(userId));
        var result = await controller.Mine();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenFound()
    {
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.GetByIdAsync(userId, orderId, false)).ReturnsAsync(new object());
        var controller = CreateController(service, CreateUser(userId));
        var result = await controller.GetById(orderId);
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenNull()
    {
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.GetByIdAsync(userId, orderId, false)).ReturnsAsync((object?)null);
        var controller = CreateController(service, CreateUser(userId));
        var result = await controller.GetById(orderId);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_ShouldPassAdminFlag_WhenAdmin()
    {
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.GetByIdAsync(userId, orderId, true)).ReturnsAsync(new object());
        var controller = CreateController(service, CreateUser(userId, true));
        var result = await controller.GetById(orderId);
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Cancel_ShouldReturnOk_WhenSuccess()
    {
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.CancelAsync(userId, orderId, false)).ReturnsAsync(true);
        var controller = CreateController(service, CreateUser(userId));
        var result = await controller.Cancel(orderId);
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Cancel_ShouldReturnNotFound_WhenFailure()
    {
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.CancelAsync(userId, orderId, false)).ReturnsAsync(false);
        var controller = CreateController(service, CreateUser(userId));
        var result = await controller.Cancel(orderId);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Cancel_ShouldPassAdminFlag_WhenAdmin()
    {
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.CancelAsync(userId, orderId, true)).ReturnsAsync(true);
        var controller = CreateController(service, CreateUser(userId, true));
        var result = await controller.Cancel(orderId);
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task All_ShouldReturnOk_WhenAdmin()
    {
        var userId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<object>());
        var controller = CreateController(service, CreateUser(userId, true));
        var result = await controller.All();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task All_ShouldReturnForbid_WhenNotAdmin()
    {
        var userId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        var controller = CreateController(service, CreateUser(userId));
        var result = await controller.All();
        result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnOk_WhenAdminAndSuccess()
    {
        var orderId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.UpdateStatusAsync(orderId, OrderStatus.Shipped)).ReturnsAsync(true);
        var controller = CreateController(service, CreateUser(Guid.NewGuid(), true));
        var dto = new UpdateOrderStatusDto { Status = OrderStatus.Shipped };
        var result = await controller.UpdateStatus(orderId, dto);
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnNotFound_WhenAdminAndFailure()
    {
        var orderId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        service.Setup(x => x.UpdateStatusAsync(orderId, OrderStatus.Shipped)).ReturnsAsync(false);
        var controller = CreateController(service, CreateUser(Guid.NewGuid(), true));
        var dto = new UpdateOrderStatusDto { Status = OrderStatus.Shipped };
        var result = await controller.UpdateStatus(orderId, dto);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnForbid_WhenNotAdmin()
    {
        var orderId = Guid.NewGuid();
        var service = new Mock<IOrderService>();
        var controller = CreateController(service, CreateUser(Guid.NewGuid()));
        var dto = new UpdateOrderStatusDto { Status = OrderStatus.Shipped };
        var result = await controller.UpdateStatus(orderId, dto);
        result.Should().BeOfType<ForbidResult>();
    }
}
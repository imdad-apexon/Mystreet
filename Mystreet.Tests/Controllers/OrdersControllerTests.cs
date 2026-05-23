using Microsoft.AspNetCore.Mvc;

namespace Mystreet.Tests.Controllers;

public class OrdersControllerTests
{
    [Fact]
    public async Task Mine_ShouldReturnOk_WhenAuthorized()
    {
        var service = new Mock<IOrderService>();
        service.Setup(x => x.GetMineAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<object>());

        var controller = new Mystreet.Api.Controllers.OrdersController(service.Object);

        controller.ControllerContext = new ControllerContext();

        var result = await controller.Mine();

        result.Should().BeOfType<OkObjectResult>();
    }
}
using Microsoft.AspNetCore.Mvc;

namespace Mystreet.Tests.Controllers;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var service = new Mock<IProductService>();
        service.Setup(x => x.GetAllAsync(null, null, null, null))
            .ReturnsAsync(new List<ProductDto>());

        var controller = new Mystreet.Api.Controllers.ProductsController(service.Object);

        var result = await controller.GetAll(null, null, null, null);

        result.Should().BeOfType<OkObjectResult>();
    }
}
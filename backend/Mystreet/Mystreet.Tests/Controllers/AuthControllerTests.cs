using Microsoft.AspNetCore.Mvc;

namespace Mystreet.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ShouldReturnOk()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequestDto>()))
            .ReturnsAsync(new AuthResponseDto { Token = "t", Email = "a@b.com" });

        var controller = new Mystreet.Api.Controllers.AuthController(auth.Object);

        var result = await controller.Register(new RegisterRequestDto());

        result.Should().BeOfType<OkObjectResult>();
    }
}
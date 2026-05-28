using Microsoft.AspNetCore.Mvc;

namespace Mystreet.Tests.Controllers;

public class AuthControllerTests
{
    #region Register Tests

    [Fact]
    public async Task Register_WithValidData_ShouldReturnOkWithToken()
    {
        // Arrange
        var auth = new Mock<IAuthService>();
        auth.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequestDto>()))
            .ReturnsAsync(new AuthResponseDto 
            { 
                Token = "jwt-token-123", 
                Email = "newuser@example.com",
                UserId = Guid.NewGuid(),
                IsAdmin = false
            });

        var controller = new Mystreet.Api.Controllers.AuthController(auth.Object);
        var registerDto = new RegisterRequestDto 
        { 
            Email = "newuser@example.com", 
            Password = "Password123!" 
        };

        // Act
        var result = await controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeOfType<AuthResponseDto>();
        var responseDto = (AuthResponseDto)okResult.Value!;
        responseDto.Token.Should().Be("jwt-token-123");
        responseDto.Email.Should().Be("newuser@example.com");
        auth.Verify(x => x.RegisterAsync(registerDto), Times.Once);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var auth = new Mock<IAuthService>();
        auth.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequestDto>()))
            .ThrowsAsync(new InvalidOperationException("Email already exists."));

        var controller = new Mystreet.Api.Controllers.AuthController(auth.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await controller.Register(new RegisterRequestDto 
            { 
                Email = "existing@example.com", 
                Password = "Password123!" 
            })
        );
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
    {
        // Arrange
        var auth = new Mock<IAuthService>();
        var userId = Guid.NewGuid();
        auth.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>()))
            .ReturnsAsync(new AuthResponseDto 
            { 
                Token = "jwt-token-456", 
                Email = "admin@example.com",
                UserId = userId,
                IsAdmin = true
            });

        var controller = new Mystreet.Api.Controllers.AuthController(auth.Object);
        var loginDto = new LoginRequestDto 
        { 
            Email = "admin@example.com", 
            Password = "Password123!" 
        };

        // Act
        var result = await controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        var responseDto = (AuthResponseDto)okResult.Value!;
        responseDto.Token.Should().Be("jwt-token-456");
        responseDto.IsAdmin.Should().BeTrue();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldThrowException()
    {
        // Arrange
        var auth = new Mock<IAuthService>();
        auth.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>()))
            .ThrowsAsync(new InvalidOperationException("Invalid credentials."));

        var controller = new Mystreet.Api.Controllers.AuthController(auth.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await controller.Login(new LoginRequestDto 
            { 
                Email = "user@example.com", 
                Password = "WrongPassword" 
            })
        );
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var auth = new Mock<IAuthService>();
        auth.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>()))
            .ThrowsAsync(new InvalidOperationException("Invalid credentials."));

        var controller = new Mystreet.Api.Controllers.AuthController(auth.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await controller.Login(new LoginRequestDto 
            { 
                Email = "nonexistent@example.com", 
                Password = "AnyPassword" 
            })
        );
    }

    #endregion
}
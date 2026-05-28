using Mystreet.Tests.Fixtures;

namespace Mystreet.Tests.Services;

public class AuthServiceTests
{
    private readonly TestDbFixture _fixture = new();

    #region Register Tests
    
    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("valid_token_123");

        var service = new AuthService(db, jwt.Object);
        var registerDto = new RegisterRequestDto
        {
            Email = "newuser@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await service.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("valid_token_123");
        result.Email.Should().Be("newuser@example.com");
        result.IsAdmin.Should().BeFalse();
        (await db.Users.CountAsync()).Should().Be(1);
        jwt.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "existing@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            IsAdmin = false
        });
        await db.SaveChangesAsync();

        var jwt = new Mock<IJwtTokenService>();
        var service = new AuthService(db, jwt.Object);

        // Act & Assert
        var act = async () => await service.RegisterAsync(new RegisterRequestDto
        {
            Email = "existing@example.com",
            Password = "DifferentPassword123!"
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already exists.");
    }

    [Fact]
    public async Task RegisterAsync_ShouldEncryptPassword()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("token");

        var service = new AuthService(db, jwt.Object);
        var plainPassword = "MySecurePassword123!";

        // Act
        var result = await service.RegisterAsync(new RegisterRequestDto
        {
            Email = "secure@example.com",
            Password = plainPassword
        });

        // Assert
        var savedUser = await db.Users.FirstAsync(u => u.Email == "secure@example.com");
        savedUser.PasswordHash.Should().NotBe(plainPassword);
        BCrypt.Net.BCrypt.Verify(plainPassword, savedUser.PasswordHash).Should().BeTrue();
    }

    #endregion

    #region Login Tests
    
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var password = "Password123!";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        
        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@example.com",
            PasswordHash = hashedPassword,
            IsAdmin = true
        });
        await db.SaveChangesAsync();

        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("jwt-token-123");

        var service = new AuthService(db, jwt.Object);

        // Act
        var result = await service.LoginAsync(new LoginRequestDto
        {
            Email = "admin@example.com",
            Password = password
        });

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token-123");
        result.IsAdmin.Should().BeTrue();
        result.Email.Should().Be("admin@example.com");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            IsAdmin = false
        });
        await db.SaveChangesAsync();

        var jwt = new Mock<IJwtTokenService>();
        var service = new AuthService(db, jwt.Object);

        // Act & Assert
        var act = async () => await service.LoginAsync(new LoginRequestDto
        {
            Email = "user@example.com",
            Password = "WrongPassword123!"
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var jwt = new Mock<IJwtTokenService>();
        var service = new AuthService(db, jwt.Object);

        // Act & Assert
        var act = async () => await service.LoginAsync(new LoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "AnyPassword123!"
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task LoginAsync_ShouldPreserveAdminStatus()
    {
        // Arrange
        await using var db = _fixture.CreateDbContext();
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@store.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPass123!"),
            IsAdmin = true
        };
        db.Users.Add(adminUser);
        await db.SaveChangesAsync();

        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("admin-token");

        var service = new AuthService(db, jwt.Object);

        // Act
        var result = await service.LoginAsync(new LoginRequestDto
        {
            Email = "admin@store.com",
            Password = "AdminPass123!"
        });

        // Assert
        result.IsAdmin.Should().BeTrue();
    }

    #endregion
}
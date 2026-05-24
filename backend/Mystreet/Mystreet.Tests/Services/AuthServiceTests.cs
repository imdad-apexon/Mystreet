using Mystreet.Tests.Fixtures;

namespace Mystreet.Tests.Services;

public class AuthServiceTests
{
    private readonly TestDbFixture _fixture = new();

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_AndReturnToken()
    {
        await using var db = _fixture.CreateDbContext();
        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("token");

        var service = new AuthService(db, jwt.Object);

        var result = await service.RegisterAsync(new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        });

        result.Token.Should().Be("token");
        result.Email.Should().Be("test@example.com");
        result.IsAdmin.Should().BeFalse();
        (await db.Users.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailExists()
    {
        await using var db = _fixture.CreateDbContext();
        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = "hash",
            IsAdmin = false
        });
        await db.SaveChangesAsync();

        var jwt = new Mock<IJwtTokenService>();
        var service = new AuthService(db, jwt.Object);

        var act = async () => await service.RegisterAsync(new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already exists.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
    {
        await using var db = _fixture.CreateDbContext();
        var password = BCrypt.Net.BCrypt.HashPassword("Password123!");
        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = password,
            IsAdmin = true
        });
        await db.SaveChangesAsync();

        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("jwt-token");

        var service = new AuthService(db, jwt.Object);

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        });

        result.Token.Should().Be("jwt-token");
        result.IsAdmin.Should().BeTrue();
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordInvalid()
    {
        await using var db = _fixture.CreateDbContext();
        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            IsAdmin = false
        });
        await db.SaveChangesAsync();

        var jwt = new Mock<IJwtTokenService>();
        var service = new AuthService(db, jwt.Object);

        var act = async () => await service.LoginAsync(new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid credentials.");
    }
}
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Mystreet.Application.DTOs.Auth;
using Mystreet.Application.Interfaces;
using Mystreet.Domain.Entities;
using Mystreet.Infrastructure.Auth;
using Mystreet.Infrastructure.Data;
using System;

namespace Mystreet.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;

    public AuthService(AppDbContext db, IJwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var email = request.Email.Trim().ToLower();
        if (await _db.Users.AnyAsync(x => x.Email == email))
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsAdmin = false
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = _jwt.CreateToken(user),
            UserId = user.Id,
            Email = user.Email,
            IsAdmin = user.IsAdmin
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var email = request.Email.Trim().ToLower();
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email)
                   ?? throw new InvalidOperationException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid credentials.");

        return new AuthResponseDto
        {
            Token = _jwt.CreateToken(user),
            UserId = user.Id,
            Email = user.Email,
            IsAdmin = user.IsAdmin
        };
    }
}
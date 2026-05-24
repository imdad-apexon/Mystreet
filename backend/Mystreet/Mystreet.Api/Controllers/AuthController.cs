using Microsoft.AspNetCore.Mvc;
using Mystreet.Application.DTOs.Auth;
using Mystreet.Application.Interfaces;

namespace Mystreet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request) => Ok(await _auth.RegisterAsync(request));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request) => Ok(await _auth.LoginAsync(request));

    [HttpPost("logout")]
    public IActionResult Logout() => Ok(new { message = "Logged out" });
}
using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (success, errorMessage) = await _authService.RegisterAsync(request);
        if (!success)
        {
            return BadRequest(errorMessage);
        }

        return Ok(new { message = "Usuario creado correctamente" });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result is null)
        {
            return Unauthorized("Credenciales inválidas.");
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new { username = User.Identity?.Name, role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value });
}

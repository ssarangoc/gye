using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsAsync(request.Username, request.Email))
        {
            return (false, "El usuario o el correo ya existen.");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Email = request.Email,
            Role = request.Role
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        var token = _jwtService.GenerateToken(user);
        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role
        };
    }
}

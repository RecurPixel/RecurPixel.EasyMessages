using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;
using WebApiSample.Models;
using WebApiSample.Services;

namespace WebApiSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and generate token
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return Msg.Validation.Failed()
                .WithHint("Username and password are required")
                .Log(_logger)
                .ToApiResponse();
        }

        var user = await _userService.AuthenticateAsync(dto);

        if (user == null)
        {
            return Msg.Auth.LoginFailed()
                .WithData(new { username = dto.Username })
                .Log(_logger)
                .ToApiResponse();
        }

        // Generate a fake token for demonstration
        var token = GenerateToken(user);

        return Msg.Auth.LoginSuccessful()
            .WithData(new { token, user = new { user.Id, user.Username, user.Email } })
            .WithMetadata("loginTime", DateTime.UtcNow)
            .Log(_logger)
            .ToApiResponse();
    }

    /// <summary>
    /// Logout (demonstration endpoint)
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // In a real app, you'd invalidate the token here
        return Msg.Auth.LogoutSuccessful()
            .Log(_logger)
            .ToApiResponse();
    }

    /// <summary>
    /// Check token validity (demonstration)
    /// </summary>
    [HttpGet("validate")]
    public IActionResult ValidateToken([FromHeader(Name = "Authorization")] string? authorization)
    {
        if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith("Bearer "))
        {
            return Msg.Auth.InvalidToken()
                .WithHint("Provide a valid Bearer token in the Authorization header")
                .Log(_logger)
                .ToApiResponse();
        }

        // Simple validation for demo purposes
        var token = authorization.Substring("Bearer ".Length);
        if (token.Length < 20)
        {
            return Msg.Auth.InvalidToken()
                .Log(_logger)
                .ToApiResponse();
        }

        return Msg.System.OperationCompleted()
            .WithData(new { valid = true, expiresIn = 3600 })
            .ToApiResponse();
    }

    private string GenerateToken(User user)
    {
        // Generate a simple fake token for demonstration
        // In a real app, use JWT or similar
        var guid = Guid.NewGuid().ToString("N");
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return $"{user.Id}_{guid}_{timestamp}";
    }
}

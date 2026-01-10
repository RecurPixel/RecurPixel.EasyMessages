using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;
using WebApiSample.Models;
using WebApiSample.Services;

namespace WebApiSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return Msg.Crud.Retrieved("Users")
            .WithData(new { users, count = users.Count() })
            .Log(_logger)
            .ToApiResponse();
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return Msg.Crud.NotFound("User")
                .WithData(new { id })
                .Log(_logger)
                .ToApiResponse();
        }

        return Msg.Crud.Retrieved("User")
            .WithData(user)
            .ToApiResponse();
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Username))
        {
            return Msg.Validation.RequiredField("username")
                .Log(_logger)
                .ToApiResponse();
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return Msg.Validation.RequiredField("email")
                .Log(_logger)
                .ToApiResponse();
        }

        // Validate email format (simple check)
        if (!dto.Email.Contains("@"))
        {
            return Msg.Validation.InvalidEmail()
                .WithData(new { email = dto.Email })
                .Log(_logger)
                .ToApiResponse();
        }

        // Validate password strength
        if (dto.Password.Length < 8)
        {
            return Msg.Validation.WeakPassword()
                .WithHint("Password must be at least 8 characters long")
                .Log(_logger)
                .ToApiResponse();
        }

        var user = await _userService.CreateAsync(dto);

        if (user == null)
        {
            // Workaround: Create custom validation error instead of using DuplicateEntry
            // which has issues with parameter substitution in some build configurations
            return Msg.Validation.Failed()
                .WithData(new {
                    field = "username/email",
                    message = "A user with this username or email already exists"
                })
                .Log(_logger)
                .ToApiResponse();
        }

        return Msg.Crud.Created("User")
            .WithData(user)
            .Log(_logger)
            .ToCreated($"/api/users/{user.Id}");
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);

        if (user == null)
        {
            return Msg.Crud.NotFound("User")
                .WithData(new { id })
                .Log(_logger)
                .ToApiResponse();
        }

        return Msg.Crud.Updated("User")
            .WithData(user)
            .Log(_logger)
            .ToApiResponse();
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _userService.DeleteAsync(id);

        if (!success)
        {
            return Msg.Crud.NotFound("User")
                .WithData(new { id })
                .Log(_logger)
                .ToApiResponse();
        }

        return Msg.Crud.Deleted("User")
            .WithData(new { id })
            .Log(_logger)
            .ToNoContent();
    }
}

using WebApiSample.Models;

namespace WebApiSample.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> CreateAsync(CreateUserDto dto);
    Task<User?> UpdateAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteAsync(int id);
    Task<User?> AuthenticateAsync(LoginDto dto);
}

public class UserService : IUserService
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public UserService()
    {
        // Seed with sample data
        _users.Add(new User
        {
            Id = _nextId++,
            Username = "admin",
            Email = "admin@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        });

        _users.Add(new User
        {
            Id = _nextId++,
            Username = "john_doe",
            Email = "john@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        });
    }

    public Task<User?> GetByIdAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<User>>(_users);
    }

    public Task<User?> CreateAsync(CreateUserDto dto)
    {
        // Simulate validation: check for duplicate username
        if (_users.Any(u => u.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase)))
        {
            return Task.FromResult<User?>(null); // Duplicate
        }

        // Simulate validation: check for duplicate email
        if (_users.Any(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return Task.FromResult<User?>(null); // Duplicate
        }

        var user = new User
        {
            Id = _nextId++,
            Username = dto.Username,
            Email = dto.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _users.Add(user);
        return Task.FromResult<User?>(user);
    }

    public Task<User?> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return Task.FromResult<User?>(null);
        }

        if (dto.Email != null)
        {
            user.Email = dto.Email;
        }

        if (dto.IsActive.HasValue)
        {
            user.IsActive = dto.IsActive.Value;
        }

        return Task.FromResult<User?>(user);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return Task.FromResult(false);
        }

        _users.Remove(user);
        return Task.FromResult(true);
    }

    public Task<User?> AuthenticateAsync(LoginDto dto)
    {
        // Simple authentication simulation (username only for demo)
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase) &&
            u.IsActive);

        return Task.FromResult(user);
    }
}

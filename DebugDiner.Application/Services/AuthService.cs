using DebugDiner.Domain.Abstractions;
using Serilog;

namespace DebugDiner.Application;

public class AuthService(IUserRepository users, ILogger logger) : IAuthService
{
    public async Task<UserEntity?> LoginAsync(string email, string password)
    {
        try
        {
            var all = await users.GetItemsAsync();
            var match = all.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (match is null)
            {
                logger.Warning("Login failed: email {Email} not found.", email);
                return null;
            }
            if (!BCrypt.Net.BCrypt.Verify(password, match.PasswordHash))
            {
                logger.Warning("Login failed: incorrect password for {Email}.", email);
                return null;
            }
            return match;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Login failed for email {Email}.", email);
            return null;
        }
    }

    public async Task<UserEntity?> RegisterAsync(string name, string email, string password)
    {
        try
        {
            var existing = await users.GetItemsAsync();
            if (existing.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                logger.Warning("Registration failed: email {Email} already in use.", email);
                return null;
            }

            var entity = new UserEntity
            {
                Id = 0,
                CreatedAt = DateTime.UtcNow,
                Name = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = Role.Customer
            };

            var created = await users.Create([entity]);
            return created.FirstOrDefault();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Registration failed for email {Email}.", email);
            return null;
        }
    }
}
namespace DebugDiner.Application;

public interface IAuthService
{
    Task<UserEntity?> LoginAsync(string email, string password);
    Task<UserEntity?> RegisterAsync(string name, string email, string password, bool isAdmin = false);
}

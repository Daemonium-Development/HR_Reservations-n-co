using DebugDiner.Application;
using DebugDiner.Domain.Abstractions;

using FluentAssertions;

using Moq;

using Serilog;

using BC = BCrypt.Net.BCrypt;

namespace DD.Application.Test;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ILogger> _logger = new();

    private AuthService CreateSut() => new(_users.Object, _logger.Object);

    private static UserEntity BuildUser(string email, string password, Role role = Role.Customer) => new()
    {
        Id = 1,
        CreatedAt = new DateTime(2024, 1, 1),
        Name = "Existing",
        Email = email,
        PasswordHash = BC.HashPassword(password),
        Role = role
    };

    // ---- Login ----

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsUser()
    {
        var user = BuildUser("user@example.com", "secret");
        _users.Setup(u => u.GetItemsAsync(null)).ReturnsAsync([user]);

        var result = await CreateSut().LoginAsync("user@example.com", "secret");

        result.Should().BeSameAs(user);
    }

    [Fact]
    public async Task LoginAsync_EmailIsCaseInsensitive_ReturnsUser()
    {
        var user = BuildUser("User@Example.com", "secret");
        _users.Setup(u => u.GetItemsAsync(null)).ReturnsAsync([user]);

        var result = await CreateSut().LoginAsync("user@example.com", "secret");

        result.Should().BeSameAs(user);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ReturnsNull()
    {
        var user = BuildUser("someone@example.com", "secret");
        _users.Setup(u => u.GetItemsAsync(null)).ReturnsAsync([user]);

        var result = await CreateSut().LoginAsync("missing@example.com", "secret");

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        var user = BuildUser("user@example.com", "secret");
        _users.Setup(u => u.GetItemsAsync(null)).ReturnsAsync([user]);

        var result = await CreateSut().LoginAsync("user@example.com", "wrong");

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_RepositoryThrows_ReturnsNull()
    {
        _users.Setup(u => u.GetItemsAsync(null)).ThrowsAsync(new InvalidOperationException("boom"));

        var result = await CreateSut().LoginAsync("user@example.com", "secret");

        result.Should().BeNull();
    }

    // ---- Register ----

    [Fact]
    public async Task RegisterAsync_NewEmail_CreatesCustomerWithHashedPassword()
    {
        _users.Setup(u => u.GetItemsAsync(null)).ReturnsAsync([]);
        UserEntity? captured = null;
        _users.Setup(u => u.Create(It.IsAny<IEnumerable<UserEntity>>()))
            .Callback<IEnumerable<UserEntity>>(entities => captured = entities.Single())
            .ReturnsAsync((IEnumerable<UserEntity> entities) => entities);

        var result = await CreateSut().RegisterAsync("Alice", "alice@example.com", "secret");

        result.Should().NotBeNull();
        captured.Should().NotBeNull();
        captured!.Role.Should().Be(Role.Customer);
        captured.Id.Should().Be(0);
        captured.Name.Should().Be("Alice");
        captured.Email.Should().Be("alice@example.com");
        captured.PasswordHash.Should().NotBe("secret");
        BC.Verify("secret", captured.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_AsAdmin_CreatesAdminUser()
    {
        _users.Setup(u => u.GetItemsAsync(null)).ReturnsAsync([]);
        UserEntity? captured = null;
        _users.Setup(u => u.Create(It.IsAny<IEnumerable<UserEntity>>()))
            .Callback<IEnumerable<UserEntity>>(entities => captured = entities.Single())
            .ReturnsAsync((IEnumerable<UserEntity> entities) => entities);

        await CreateSut().RegisterAsync("Admin", "admin@example.com", "secret", isAdmin: true);

        captured.Should().NotBeNull();
        captured!.Role.Should().Be(Role.Admin);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmailCaseInsensitive_ReturnsNullAndDoesNotCreate()
    {
        var existing = BuildUser("User@Example.com", "secret");
        _users.Setup(u => u.GetItemsAsync(null)).ReturnsAsync([existing]);

        var result = await CreateSut().RegisterAsync("Bob", "user@example.com", "secret");

        result.Should().BeNull();
        _users.Verify(u => u.Create(It.IsAny<IEnumerable<UserEntity>>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_RepositoryThrows_ReturnsNull()
    {
        _users.Setup(u => u.GetItemsAsync(null)).ReturnsAsync([]);
        _users.Setup(u => u.Create(It.IsAny<IEnumerable<UserEntity>>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var result = await CreateSut().RegisterAsync("Alice", "alice@example.com", "secret");

        result.Should().BeNull();
    }
}

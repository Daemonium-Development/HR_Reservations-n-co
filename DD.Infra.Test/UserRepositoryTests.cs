using DebugDiner.Infrastructure.Repositories;

using FluentAssertions;

using Xunit.Abstractions;

using BC = BCrypt.Net.BCrypt;

namespace DD.Infra.Test;

[TestCaseOrderer("DD.Infra.Test.PriorityOrderer", "DD.Infra.Test")]
public class UserRepositoryTests(DatabaseFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<DatabaseFixture>
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    private readonly UserRepository _repository = fixture.GetUserRepository();

    [Fact, TestPriority(1)]
    public async Task GetSingleUser_ShouldSucceed()
    {
        var entity = await _repository.GetItemsAsync([1]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(2)]
    public async Task GetMultipleUsers_ShouldGiveOnlyOne()
    {
        var entity = await _repository.GetItemsAsync([1, 2]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(3)]
    public async Task GetSingleUser_ShouldFail()
    {
        var entity = await _repository.GetItemsAsync([2]);

        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(4)]
    public async Task GetMultipleUsers_ShouldFail()
    {
        var entity = await _repository.GetItemsAsync([2, 3]);

        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(5)]
    public async Task GetAllUsers_ShouldSucceed()
    {
        var entity = await _repository.GetItemsAsync();

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(6)]
    public async Task CreateUser_ShouldSucceed()
    {
        var user = new UserEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Test User",
            Email = "test@example.com",
            PasswordHash = BC.HashPassword("password"),
            Role = Role.Customer
        };

        var entity = await _repository.Create([user]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(7)]
    public async Task UpdateUser_ShouldSucceed()
    {
        var user = new UserEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Test User AfterUpdate",
            Email = "test@example.com",
            PasswordHash = BC.HashPassword("password2"),
            Role = Role.Employee
        };

        var entities = await _repository.Update([user]);

        entities.Should().NotBeNull().And.HaveCount(1);
        var entity = entities.First();
        entity.Id.Should().Be(2);
        entity.Name.Should().Be(user.Name).And.NotBe("Test User");
        entity.Role.Should().Be(user.Role);
    }

    [Fact, TestPriority(8)]
    public async Task DeleteUser_ShouldReduceCount()
    {
        var items = await _repository.GetItemsAsync();
        _ = await _repository.Delete(items);

        items = await _repository.GetItemsAsync();
        items.Should().NotBeNull().And.HaveCount(0);
    }
}

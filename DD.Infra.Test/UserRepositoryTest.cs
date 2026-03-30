// using DebugDiner.Domain.Configurations;
// using DebugDiner.Infrastructure.Repositories;
// using DebugDiner.Infrastructure.Services;
// using FluentAssertions;
// using Microsoft.Extensions.Options;
// using Moq;
// using Xunit.Abstractions;
// using BC = BCrypt.Net.BCrypt;
// using ILogger = Serilog.ILogger;
//
//
// namespace DD.Infra.Test;
//
// public class UserRepositoryTest
// {
//     private readonly ITestOutputHelper _testOutputHelper;
//     private readonly UserRepository _repository;
//     private DataService _database;
//     
//     public UserRepositoryTest(ITestOutputHelper testOutputHelper)
//     {
//         _testOutputHelper = testOutputHelper;
//         var options = Options.Create(new DatabaseOptions { Source = "test.db" });
//         var mockLogger = new Mock<ILogger>();
//         _database = new DataService(options, mockLogger.Object);
//         _repository = new UserRepository(mockLogger.Object);
//         try
//         {
//             var connection = _database.StartAsync().Result;
//             _repository.Connection = connection;
//         }
//         catch (Exception ex)
//         {
//             _testOutputHelper.WriteLine(ex.ToString());
//         }
//     }
//
//     [Fact]
//     public async Task GetSingleUser_ShouldSucceed()
//     {
//         // Act
//         var user = await _repository.GetItemsAsync([1]);
//         
//         // Assert
//         user.Count().Should().Be(1);
//         user.First().Id.Should().Be(1);
//     }
//
//     [Fact]
//     public async Task GetMultipleUsers_ShouldGiveOnlyOne()
//     {
//         // Act
//         var users = await _repository.GetItemsAsync([1, 2]);
//         
//         // Assert
//         users.Count().Should().Be(1);
//         users.First().Id.Should().Be(1);
//     }
//
//     [Fact]
//     public async Task GetSingleUser_ShouldFail()
//     {
//         // Act
//         var user = await _repository.GetItemsAsync([2]);
//         
//         // Assert
//         user.Count().Should().Be(0);
//     }
//
//     [Fact]
//     public async Task GetMultipleUsers_ShouldFail()
//     {
//         // Act
//         var user = await _repository.GetItemsAsync([2, 3]);
//         
//         // Assert
//         user.Count().Should().Be(0);
//     }
//
//     [Fact]
//     public async Task GetAllUsers_ShouldSucceed()
//     {
//         var users = await _repository.GetItemsAsync();
//         users.Count().Should().Be(1);
//     }
//
//     [Fact]
//     public async Task CreateUser_ShouldSucceed()
//     {
//         var user = new UserEntity
//         {
//             Id = 2, // Leave null to auto-increment
//             CreatedAt = DateTime.Now, // Default value
//             UpdatedAt = DateTime.Now,
//             Name = "Test_second",
//             Email = "Test@second.com",
//             PasswordHash = BC.HashPassword("password"),
//             Role = Role.Admin
//         };
//
//         var created = await _repository.Create([user]);
//         created.Should().BeEqualTo(user);
//     }
//
//     [Fact]
//     public async Task UpdateUser_ShouldSucceed()
//     {
//         var user = new UserEntity
//         {
//             Id = 2, // Leave null to auto-increment
//             CreatedAt = DateTime.Now, // Default value
//             UpdatedAt = DateTime.Now,
//             Name = "Test_second",
//             Email = "Test@second.com",
//             PasswordHash = BC.HashPassword("password2"),
//             Role = Role.Admin
//         };
//
//         var updated = await _repository.Create([user]);
//         updated.Should().BeEqualTo(updated);
//     }
//
//     [Fact]
//     public async Task DeleteUser_ShouldReduceCount()
//     {
//         var user = new UserEntity
//         {
//             Id = 2, // Will be ignored due to auto-increment
//             CreatedAt = DateTime.Now, // Will be ignored
//             UpdatedAt = DateTime.Now,
//             Name = "Test_second",
//             Email = "Test@second.com",
//             PasswordHash = BC.HashPassword("password2"),
//             Role = Role.Admin
//         };
//         
//         _ = await _repository.Create([user]);
//         
//         var currentList = await _repository.GetItemsAsync();
//         var count = currentList.Count();
//         count.Should().Be(2);
//         
//         var deleted = await _repository.Delete([user]);
//         
//         var newList = await _repository.GetItemsAsync();
//         newList.Count().Should().Be(count - deleted);
//     }
// }
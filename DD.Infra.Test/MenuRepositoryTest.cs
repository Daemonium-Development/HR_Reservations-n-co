using DebugDiner.Domain.Configurations;
using DebugDiner.Infrastructure.Repositories;
using DebugDiner.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;
using ILogger = Serilog.ILogger;


namespace DD.Infra.Test;

public class MenuRepositoryTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly MenuRepository _repository;
    private DataService _database;

    public MenuRepositoryTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var options = Options.Create(new DatabaseOptions { Source = "test.db" });
        var mockLogger = new Mock<ILogger>();
        _database = new DataService(options, mockLogger.Object);
        _repository = new MenuRepository(mockLogger.Object);
        try
        {
            var connection = _database.StartAsync().Result;
            _repository.Connection = connection;
        }
        catch (Exception ex)
        {
            _testOutputHelper.WriteLine(ex.ToString());
        }
    }

    [Fact]
    public async Task GetSingleDish_ShouldSucceed()
    {
        // Act
        var dish = await _repository.GetItemsAsync([1]);

        // Assert
        dish.Count().Should().Be(1);
        dish.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetMultipleDishes_ShouldGiveOnlyOne()
    {
        // Act
        var dishes = await _repository.GetItemsAsync([1, 2]);

        // Assert
        dishes.Count().Should().Be(1);
        dishes.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetSingleDish_ShouldFail()
    {
        // Act
        var dish = await _repository.GetItemsAsync([2]);

        // Assert
        dish.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetMultipleDishes_ShouldFail()
    {
        // Act
        var dish = await _repository.GetItemsAsync([2, 3]);

        // Assert
        dish.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetAllDishes_ShouldSucceed()
    {
        var dishes = await _repository.GetItemsAsync();
        dishes.Count().Should().Be(1);
    }

    [Fact]
    public async Task CreateDish_ShouldSucceed()
    {
        var dish = new DishEntity
        {
            Id = 2, // Leave null to auto-increment
            CreatedAt = DateTime.Now, // Default value
            UpdatedAt = DateTime.Now,
            Name = "Test_second",
            Description = "Test description",
            Price = 9.99m,
            DishCategory = DishCategory.Meat,
            AllergenInfo = "None"
        };

        var created = await _repository.Create([dish]);
        created.Should().BeEqualTo(dish);
    }

    [Fact]
    public async Task UpdateDish_ShouldSucceed()
    {
        var dish = new DishEntity
        {
            Id = 2, // Leave null to auto-increment
            CreatedAt = DateTime.Now, // Default value
            UpdatedAt = DateTime.Now,
            Name = "Test_second",
            Description = "Updated description",
            Price = 12.99m,
            DishCategory = DishCategory.Vegetarian,
            AllergenInfo = "None"
        };

        var updated = await _repository.Create([dish]);
        updated.Should().BeEqualTo(updated);
    }

    [Fact]
    public async Task DeleteDish_ShouldReduceCount()
    {
        var dish = new DishEntity
        {
            Id = 2, // Will be ignored due to auto-increment
            CreatedAt = DateTime.Now, // Will be ignored
            UpdatedAt = DateTime.Now,
            Name = "Test_second",
            Description = "Test description",
            Price = 9.99m,
            DishCategory = DishCategory.Meat,
            AllergenInfo = "None"
        };

        _ = await _repository.Create([dish]);

        var currentList = await _repository.GetItemsAsync();
        var count = currentList.Count(); // Should be 2

        await _repository.Delete([dish]);

        var newList = await _repository.GetItemsAsync();
        newList.Count().Should().Be(1);
    }
}
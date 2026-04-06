using DebugDiner.Infrastructure.Repositories;

using FluentAssertions;

using Xunit.Abstractions;

namespace DD.Infra.Test;

[TestCaseOrderer("DD.Infra.Test.PriorityOrderer", "DD.Infra.Test")]
public class MenuRepositoryTests(DatabaseFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<DatabaseFixture>
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    private readonly MenuRepository _repository = fixture.GetMenuRepository();

    [Fact, TestPriority(1)]
    public async Task GetSingleDish_ShouldSucceed()
    {
        var entity = await _repository.GetItemsAsync([1]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(2)]
    public async Task GetMultipleDishes_ShouldGiveOnlyOne()
    {
        var entity = await _repository.GetItemsAsync([1, 2]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(3)]
    public async Task GetSingleDish_ShouldFail()
    {
        var entity = await _repository.GetItemsAsync([2]);

        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(4)]
    public async Task GetMultipleDishes_ShouldFail()
    {
        var entity = await _repository.GetItemsAsync([2, 3]);

        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(5)]
    public async Task GetAllDishes_ShouldSucceed()
    {
        var entity = await _repository.GetItemsAsync();

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(6)]
    public async Task CreateDish_ShouldSucceed()
    {
        var dish = new DishEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Test Dish",
            Description = "Test description",
            Price = 9.99m,
            DishCategory = DishCategory.Meat,
            AllergenInfo = "None"
        };

        var entity = await _repository.Create([dish]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(7)]
    public async Task UpdateDish_ShouldSucceed()
    {
        var dish = new DishEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Test Dish AfterUpdate",
            Description = "Updated description",
            Price = 12.99m,
            DishCategory = DishCategory.Vegetarian,
            AllergenInfo = "None"
        };

        var entities = await _repository.Update([dish]);

        entities.Should().NotBeNull().And.HaveCount(1);
        var entity = entities.First();
        entity.Id.Should().Be(2);
        entity.Name.Should().Be(dish.Name).And.NotBe("Test Dish");
        entity.Price.Should().Be(dish.Price);
        entity.DishCategory.Should().Be(dish.DishCategory);
    }

    [Fact, TestPriority(8)]
    public async Task DeleteDish_ShouldReduceCount()
    {
        var item = await _repository.GetItemsAsync([2]);
        _ = await _repository.Delete(item);

        var items = await _repository.GetItemsAsync();
        items.Should().NotBeNull().And.HaveCount(1);
    }
}

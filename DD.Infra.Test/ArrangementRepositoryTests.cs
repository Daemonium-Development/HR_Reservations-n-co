using DebugDiner.Infrastructure.Repositories;

using FluentAssertions;

using Xunit.Abstractions;

namespace DD.Infra.Test;

[TestCaseOrderer("DD.Infra.Test.PriorityOrderer", "DD.Infra.Test")]
public class ArrangementRepositoryTests(DatabaseFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<DatabaseFixture>
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    private readonly ArrangementRepository _repository = fixture.GetArrangementRepository();

    [Fact, TestPriority(1)]
    public async Task GetSingleArrangement_ShouldSucceed()
    {
        // Arrange
        var entity = await _repository.GetItemsAsync([1]);

        // Act

        // Assert
        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(2)]
    public async Task GetMultipleArrangement_ShouldGiveOnlyOne()
    {
        // Arrange
        var entity = await _repository.GetItemsAsync([1, 2]);

        // Act

        // Assert
        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(3)]
    public async Task GetSingleArrangement_ShouldFail()
    {
        // Arrange
        var entity = await _repository.GetItemsAsync([2]);

        // Act

        // Assert
        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(4)]
    public async Task GetMultipleArrangements_ShouldFail()
    {
        // Arrange
        var entity = await _repository.GetItemsAsync([2, 3]);

        // Act

        // Assert
        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(5)]
    public async Task GetAllArrangements_ShouldSucceed()
    {
        // Arrange
        var entity = await _repository.GetItemsAsync();

        // Act

        // Assert
        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(6)]
    public async Task CreateArrangement_ShouldSucceed()
    {
        // Arrange
        var arrangement = new ArrangementEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Test Arrangement",
            BasePrice = 9.99m,
            Type = ArrangementType.TwoCourse
        };

        // Act
        var entity = await _repository.Create([arrangement]);

        // Assert
        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(7)]
    public async Task UpdateArrangement_ShouldSucceed()
    {
        // Arrange
        var arrangement = new ArrangementEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Name = "Test Arrangement AfterUpdate",
            BasePrice = 9.99m,
            Type = ArrangementType.TwoCourse
        };

        // Act
        var entities = await _repository.Update([arrangement]);

        // Assert
        entities.Should().NotBeNull().And.HaveCount(1);
        var entity = entities.First();
        entity.Id.Should().Be(2);
        entity.Name.Should().Be(arrangement.Name).And.NotBe("Test Arrangement");
        entity.BasePrice.Should().Be(arrangement.BasePrice);
        entity.Type.Should().Be(arrangement.Type);
    }

    [Fact, TestPriority(8)]
    public async Task DeleteArrangement_ShouldReduceCount()
    {
        // Arrange
        var item = await _repository.GetItemsAsync([2]);

        // Act
        _ = await _repository.Delete(item);

        var items = await _repository.GetItemsAsync();
        items.Should().NotBeNull().And.HaveCount(1);
    }
}

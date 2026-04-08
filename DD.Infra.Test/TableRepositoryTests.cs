using DebugDiner.Infrastructure.Repositories;

using FluentAssertions;

using Xunit.Abstractions;

namespace DD.Infra.Test;

[TestCaseOrderer("DD.Infra.Test.PriorityOrderer", "DD.Infra.Test")]
public class TableRepositoryTests(DatabaseFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<DatabaseFixture>
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    private readonly TableRepository _repository = fixture.GetTableRepository();

    [Fact, TestPriority(1)]
    public async Task GetSingleTable_ShouldSucceed()
    {
        var entity = await _repository.GetItemsAsync([1]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(2)]
    public async Task GetMultipleTables_ShouldGiveOnlyOne()
    {
        var entity = await _repository.GetItemsAsync([1, 2]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(3)]
    public async Task GetSingleTable_ShouldFail()
    {
        var entity = await _repository.GetItemsAsync([2]);

        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(4)]
    public async Task GetMultipleTables_ShouldFail()
    {
        var entity = await _repository.GetItemsAsync([2, 3]);

        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(5)]
    public async Task GetAllTables_ShouldSucceed()
    {
        var entity = await _repository.GetItemsAsync();

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(6)]
    public async Task CreateTable_ShouldSucceed()
    {
        var table = new TableEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Capacity = 4,
            Type = TableType.FourPerson
        };

        var entity = await _repository.Create([table]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(7)]
    public async Task UpdateTable_ShouldSucceed()
    {
        var table = new TableEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Capacity = 6,
            Type = TableType.SixPerson
        };

        var entities = await _repository.Update([table]);

        entities.Should().NotBeNull().And.HaveCount(1);
        var entity = entities.First();
        entity.Id.Should().Be(2);
        entity.Capacity.Should().Be(table.Capacity);
        entity.Type.Should().Be(table.Type);
    }

    [Fact, TestPriority(8)]
    public async Task DeleteTable_ShouldReduceCount()
    {
        var items = await _repository.GetItemsAsync();
        _ = await _repository.Delete(items);

        items = await _repository.GetItemsAsync();
        items.Should().NotBeNull().And.HaveCount(0);
    }
}

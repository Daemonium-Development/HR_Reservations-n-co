using DebugDiner.Domain.Configurations;
using DebugDiner.Infrastructure.Repositories;
using DebugDiner.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;
using ILogger = Serilog.ILogger;


namespace DD.Infra.Test;

public class TableRepositoryTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly TableRepository _repository;
    private DataService _database;

    public TableRepositoryTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var options = Options.Create(new DatabaseOptions { Source = "test.db" });
        var mockLogger = new Mock<ILogger>();
        _database = new DataService(options, mockLogger.Object);
        _repository = new TableRepository(mockLogger.Object);
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
    public async Task GetSingleTable_ShouldSucceed()
    {
        // Act
        var table = await _repository.GetItemsAsync([1]);

        // Assert
        table.Count().Should().Be(1);
        table.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetMultipleTables_ShouldGiveOnlyOne()
    {
        // Act
        var tables = await _repository.GetItemsAsync([1, 2]);

        // Assert
        tables.Count().Should().Be(1);
        tables.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetSingleTable_ShouldFail()
    {
        // Act
        var table = await _repository.GetItemsAsync([2]);

        // Assert
        table.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetMultipleTables_ShouldFail()
    {
        // Act
        var table = await _repository.GetItemsAsync([2, 3]);

        // Assert
        table.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetAllTables_ShouldSucceed()
    {
        var tables = await _repository.GetItemsAsync();
        tables.Count().Should().Be(1);
    }

    [Fact]
    public async Task CreateTable_ShouldSucceed()
    {
        var table = new TableEntity
        {
            Id = 2, // Leave null to auto-increment
            CreatedAt = DateTime.Now, // Default value
            UpdatedAt = DateTime.Now,
            Capacity = 4,
            Type = TableType.FourPerson
        };

        var created = await _repository.Create([table]);
        created.Should().BeEqualTo(table);
    }

    [Fact]
    public async Task UpdateTable_ShouldSucceed()
    {
        var table = new TableEntity
        {
            Id = 2, // Leave null to auto-increment
            CreatedAt = DateTime.Now, // Default value
            UpdatedAt = DateTime.Now,
            Capacity = 6,
            Type = TableType.SixPerson
        };

        var updated = await _repository.Create([table]);
        updated.Should().BeEqualTo(updated);
    }

    [Fact]
    public async Task DeleteTable_ShouldReduceCount()
    {
        var preList = await _repository.GetItemsAsync();
        var count = preList.Count();
        count.Should().Be(1);
        
        var table = new TableEntity
        {
            Id = 2, // Will be ignored due to auto-increment
            CreatedAt = DateTime.Now, // Will be ignored
            UpdatedAt = DateTime.Now,
            Capacity = 4,
            Type = TableType.FourPerson
        };

        _ = await _repository.Create([table]);

        var currentList = await _repository.GetItemsAsync();
        count = currentList.Count();
        count.Should().Be(2);
        
        var deleted = await _repository.Delete([table]);

        var newList = await _repository.GetItemsAsync();
        newList.Count().Should().Be(count - deleted);
    }
}
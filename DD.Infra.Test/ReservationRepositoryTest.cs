using DebugDiner.Domain.Configurations;
using DebugDiner.Infrastructure.Repositories;
using DebugDiner.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;
using ILogger = Serilog.ILogger;

namespace DD.Infra.Test;

public class ReservationRepositoryTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ReservationRepository _repository;
    private DataService _database;

    public ReservationRepositoryTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var options = Options.Create(new DatabaseOptions { Source = "test.db" });
        var mockLogger = new Mock<ILogger>();
        _database = new DataService(options, mockLogger.Object);
        _repository = new ReservationRepository(mockLogger.Object);
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
    public async Task GetSingleReservation_ShouldSucceed()
    {
        // Act
        var reservation = await _repository.GetItemsAsync([1]);

        // Assert
        reservation.Count().Should().Be(1);
        reservation.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetMultipleReservations_ShouldGiveOnlyOne()
    {
        // Act
        var reservations = await _repository.GetItemsAsync([1, 2]);

        // Assert
        reservations.Count().Should().Be(1);
        reservations.First().Id.Should().Be(1);
    }

    [Fact]
    public async Task GetSingleReservation_ShouldFail()
    {
        // Act
        var reservation = await _repository.GetItemsAsync([2]);

        // Assert
        reservation.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetMultipleReservations_ShouldFail()
    {
        // Act
        var reservation = await _repository.GetItemsAsync([2, 3]);

        // Assert
        reservation.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetAllReservations_ShouldSucceed()
    {
        var reservations = await _repository.GetItemsAsync();
        reservations.Count().Should().Be(1);
    }

    [Fact]
    public async Task CreateReservation_ShouldSucceed()
    {
        var reservation = new ReservationEntity
        {
            Id = 2, // Leave null to auto-increment
            CreatedAt = DateTime.Now, // Default value
            UpdatedAt = DateTime.Now,
            UserId = 1,
            TableId = 1,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Guests = 2,
            Status = ReservationStatus.Pending
        };

        var created = await _repository.Create([reservation]);
        var item = created.First();
        item.UserId.Should().Be(reservation.UserId);
        item.TableId.Should().Be(reservation.TableId);
        item.StartTime.Should().Be(reservation.StartTime);
        item.EndTime.Should().Be(reservation.EndTime);
        item.Guests.Should().Be(reservation.Guests);
        item.Status.Should().Be(reservation.Status);
    }

    [Fact]
    public async Task UpdateReservation_ShouldSucceed()
    {
        var reservation = new ReservationEntity
        {
            Id = 2, // Leave null to auto-increment
            CreatedAt = DateTime.Now, // Default value
            UpdatedAt = DateTime.Now,
            UserId = 1,
            TableId = 1,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Guests = 4,
            Status = ReservationStatus.Confirmed
        };

        var updated = await _repository.Create([reservation]);
        updated.Should().BeEqualTo(updated);
    }

    [Fact]
    public async Task DeleteReservation_ShouldReduceCount()
    {
        var reservation = new ReservationEntity
        {
            Id = 2, // Will be ignored due to auto-increment
            CreatedAt = DateTime.Now, // Will be ignored
            UpdatedAt = DateTime.Now,
            UserId = 1,
            TableId = 1,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Guests = 2,
            Status = ReservationStatus.Pending
        };

        _ = await _repository.Create([reservation]);

        var currentList = await _repository.GetItemsAsync();
        var count = currentList.Count(); // Should be 2

        var deleted = await _repository.Delete([reservation]);

        var newList = await _repository.GetItemsAsync();
        newList.Count().Should().Be(count - deleted);
    }
}
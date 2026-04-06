using DebugDiner.Infrastructure.Repositories;

using FluentAssertions;

using Xunit.Abstractions;

namespace DD.Infra.Test;

[TestCaseOrderer("DD.Infra.Test.PriorityOrderer", "DD.Infra.Test")]
public class ReservationRepositoryTests(DatabaseFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<DatabaseFixture>
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    private readonly ReservationRepository _repository = fixture.GetReservationRepository();

    [Fact, TestPriority(1)]
    public async Task GetSingleReservation_ShouldSucceed()
    {
        var entity = await _repository.GetItemsAsync([1]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(2)]
    public async Task GetMultipleReservations_ShouldGiveOnlyOne()
    {
        var entity = await _repository.GetItemsAsync([1, 2]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(3)]
    public async Task GetSingleReservation_ShouldFail()
    {
        var entity = await _repository.GetItemsAsync([2]);

        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(4)]
    public async Task GetMultipleReservations_ShouldFail()
    {
        var entity = await _repository.GetItemsAsync([2, 3]);

        entity.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact, TestPriority(5)]
    public async Task GetAllReservations_ShouldSucceed()
    {
        var entity = await _repository.GetItemsAsync();

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(6)]
    public async Task CreateReservation_ShouldSucceed()
    {
        var reservation = new ReservationEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UserId = 1,
            TableId = 1,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Guests = 2,
            Status = ReservationStatus.Pending
        };

        var entity = await _repository.Create([reservation]);

        entity.Should().NotBeNull().And.HaveCount(1);
    }

    [Fact, TestPriority(7)]
    public async Task UpdateReservation_ShouldSucceed()
    {
        var reservation = new ReservationEntity
        {
            Id = 2,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UserId = 1,
            TableId = 1,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Guests = 4,
            Status = ReservationStatus.Confirmed
        };

        var entities = await _repository.Update([reservation]);

        entities.Should().NotBeNull().And.HaveCount(1);
        var entity = entities.First();
        entity.Id.Should().Be(2);
        entity.Guests.Should().Be(reservation.Guests);
        entity.Status.Should().Be(reservation.Status);
    }

    [Fact, TestPriority(8)]
    public async Task DeleteReservation_ShouldReduceCount()
    {
        var item = await _repository.GetItemsAsync([2]);
        _ = await _repository.Delete(item);

        var items = await _repository.GetItemsAsync();
        items.Should().NotBeNull().And.HaveCount(1);
    }
}

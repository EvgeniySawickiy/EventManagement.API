
using EventManagement.Application.Use_Cases.EventUseCases;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Moq;
using Xunit;

namespace EventManagement.Tests
{
    public class GetEventsByCriteriaUseCaseTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly GetEventsByCriteriaUseCase _getEventsByCriteriaUseCase;

        public GetEventsByCriteriaUseCaseTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _getEventsByCriteriaUseCase = new GetEventsByCriteriaUseCase(_unitOfWorkMock.Object);
        }

        [Fact]
        public void ExecuteAsync_WhenCriteriaMatch_ReturnsFilteredEvents()
        {
            // Arrange
            var date = new DateTime(2024, 10, 19);
            var location = "New York";
            var category = "Music";

            var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), EventDate = date, Location = "New York", Category = "Music" },
            new Event { Id = Guid.NewGuid(), EventDate = date.AddDays(1), Location = "Los Angeles", Category = "Art" },
            new Event { Id = Guid.NewGuid(), EventDate = date, Location = "New York", Category = "Art" },
        };

            _unitOfWorkMock.Setup(u => u.Events.Query()).Returns(events.AsQueryable());

            // Act
            var result = _getEventsByCriteriaUseCase.ExecuteAsync(date, location, category).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(events[0].Id, result[0].Id);
        }

        [Fact]
        public void ExecuteAsync_WhenNoEventsMatch_ReturnsEmptyList()
        {
            // Arrange
            var date = new DateTime(2024, 10, 19);
            var location = "Unknown Location";
            var category = "Unknown Category";

            var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), EventDate = date, Location = "New York", Category = "Music" },
            new Event { Id = Guid.NewGuid(), EventDate = date.AddDays(1), Location = "Los Angeles", Category = "Art" },
        };

            _unitOfWorkMock.Setup(u => u.Events.Query()).Returns(events.AsQueryable());

            // Act
            var result = _getEventsByCriteriaUseCase.ExecuteAsync(date, location, category).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ExecuteAsync_WhenDateIsNull_ReturnsAllEventsByLocationAndCategory()
        {
            // Arrange
            var location = "New York";
            var category = "Music";

            var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), EventDate = DateTime.Now, Location = "New York", Category = "Music" },
            new Event { Id = Guid.NewGuid(), EventDate = DateTime.Now, Location = "Los Angeles", Category = "Music" },
            new Event { Id = Guid.NewGuid(), EventDate = DateTime.Now, Location = "New York", Category = "Art" },
        };

            _unitOfWorkMock.Setup(u => u.Events.Query()).Returns(events.AsQueryable());

            // Act
            var result = _getEventsByCriteriaUseCase.ExecuteAsync(null, location, category).ToList();

            // Assert
            Assert.Single(result);
            Assert.All(result, e => Assert.Equal(category, e.Category));
            Assert.All(result, e => Assert.Contains(location, e.Location));
        }

        [Fact]
        public void ExecuteAsync_WhenLocationIsEmpty_ReturnsEventsByDateAndCategory()
        {
            // Arrange
            var date = new DateTime(2024, 10, 19);
            var category = "Music";

            var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), EventDate = date, Location = "New York", Category = "Music" },
            new Event { Id = Guid.NewGuid(), EventDate = date, Location = "Los Angeles", Category = "Music" },
            new Event { Id = Guid.NewGuid(), EventDate = date, Location = "New York", Category = "Art" },
        };

            _unitOfWorkMock.Setup(u => u.Events.Query()).Returns(events.AsQueryable());

            // Act
            var result = _getEventsByCriteriaUseCase.ExecuteAsync(date, string.Empty, category).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, e => Assert.Equal(date, e.EventDate.Date));
            Assert.All(result, e => Assert.Equal(category, e.Category));
        }
    }
}

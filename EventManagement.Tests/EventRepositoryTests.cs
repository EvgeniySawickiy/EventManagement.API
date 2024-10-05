using EventManagement.Core.Entity;
using EventManagement.DataAccess;
using EventManagement.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace EventManagement.Tests
{
    public class EventRepositoryTests
    {
        private readonly Mock<EventDbContext> _mockContext;
        private readonly EventRepository _eventRepository;

        public EventRepositoryTests()
        {
            _mockContext = new Mock<EventDbContext>();
            _eventRepository = new EventRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {
            // Arrange
            var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Name = "Event 1" },
            new Event { Id = Guid.NewGuid(), Name = "Event 2" }
        }.AsQueryable();

            _mockContext.Setup(c => c.Events).ReturnsDbSet(events);

            // Act
            var result = await _eventRepository.GetAllEventsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var expectedEvent = new Event { Id = eventId, Name = "Test Event" };

            _mockContext.Setup(c => c.Events.FindAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var actualEvent = await _eventRepository.GetEventByIdAsync(eventId);

            // Assert
            Assert.NotNull(actualEvent);
            Assert.Equal(expectedEvent.Id, actualEvent.Id);
        }

        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnNull_WhenEventDoesNotExist()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _mockContext.Setup(c => c.Events.FindAsync(eventId))
                .ReturnsAsync((Event)null);

            // Act
            var actualEvent = await _eventRepository.GetEventByIdAsync(eventId);

            // Assert
            Assert.Null(actualEvent);
        }

        [Fact]
        public async Task AddEventAsync_ShouldSaveEvent_WhenEventIsValid()
        {
            // Arrange
            var newEvent = new Event { Id = Guid.NewGuid(), Name = "New Event" };

            _mockContext.Setup(c => c.Events.AddAsync(newEvent, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(newEvent));

            // Act
            await _eventRepository.AddEventAsync(newEvent);

            // Assert
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateEventAsync_ShouldUpdateEvent_WhenEventExists()
        {
            // Arrange
            var existingEvent = new Event { Id = Guid.NewGuid(), Name = "Existing Event" };

            _mockContext.Setup(c => c.Events.Update(existingEvent));

            // Act
            await _eventRepository.UpdateEventAsync(existingEvent);

            // Assert
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteEventAsync_ShouldRemoveEvent_WhenEventExists()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventToDelete = new Event { Id = eventId };

            _mockContext.Setup(c => c.Events.FindAsync(eventId)).ReturnsAsync(eventToDelete);

            // Act
            await _eventRepository.DeleteEventAsync(eventId);

            // Assert
            _mockContext.Verify(c => c.Remove(eventToDelete), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetEventByNameAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var eventName = "Test Event";
            var expectedEvent = new Event { Id = Guid.NewGuid(), Name = eventName };

            _mockContext.Setup(c => c.Events.FirstOrDefaultAsync(It.IsAny<Expression<Func<Event, bool>>>()))
                .ReturnsAsync(expectedEvent);

            // Act
            var actualEvent = await _eventRepository.GetEventByNameAsync(eventName);

            // Assert
            Assert.NotNull(actualEvent);
            Assert.Equal(expectedEvent.Name, actualEvent.Name);
        }

        [Fact]
        public async Task GetEventsByCriteriaAsync_ShouldReturnFilteredEvents()
        {
            // Arrange
            var eventList = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), EventDate = DateTime.Now, Location = "Location A", Category = "Category 1" },
            new Event { Id = Guid.NewGuid(), EventDate = DateTime.Now.AddDays(1), Location = "Location B", Category = "Category 2" }
        }.AsQueryable();

            _mockContext.Setup(c => c.Events).ReturnsDbSet(eventList);

            // Act
            var result = await _eventRepository.GetEventsByCriteriaAsync(DateTime.Now, "Location A", "Category 1");

            // Assert
            Assert.Single(result);
            Assert.Equal("Location A", result.First().Location);
        }
    }
}
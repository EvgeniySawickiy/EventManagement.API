using EventManagement.Core.Entity;
using EventManagement.DataAccess;
using EventManagement.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EventManagement.Tests
{
    public class EventRepositoryTests
    {
        private readonly EventDbContext _context;
        private readonly EventRepository _repository;

        public EventRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new EventDbContext(options);
            _repository = new EventRepository(_context);
        }

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {
            // Arrange
            _context.Events.Add(new Event { Id = Guid.NewGuid(), Name = "Event1", Category = "New Category", Description = "Description", Location = "New Location" });
            _context.Events.Add(new Event { Id = Guid.NewGuid(), Name = "Event2", Category = "New Category", Description = "Description", Location = "New Location" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllEventsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddEventAsync_ShouldAddEventToDatabase()
        {
            // Arrange
            var newEvent = new Event { Id = Guid.NewGuid(), Name = "New Event", Category="New Category", Description="Description",Location="New Location" };

            // Act
            await _repository.AddEventAsync(newEvent);

            // Assert
            var addedEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == newEvent.Id);
            Assert.NotNull(addedEvent);
            Assert.Equal("New Event", addedEvent.Name);
        }


        [Fact]
        public async Task GetEventByIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _context.Events.Add(new Event { Id = eventId, Name = "Test Event", Category = "New Category", Description = "Description", Location = "New Location" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetEventByIdAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Event", result.Name);
        }
    }
}
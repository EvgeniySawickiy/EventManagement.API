using EventManagement.Core.Entity;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EventManagement.Tests
{
    public class EventRepositoryTests
    {
        private readonly EventDbContext _context;
        private readonly EventRepository _eventRepository;

        public EventRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseInMemoryDatabase(databaseName: "EventDatabase")
                .Options;

            _context = new EventDbContext(options);
            _eventRepository = new EventRepository(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsEvent()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventEntity = new Event
            {
                Id = eventId,
                Name = "Test Event",
                Description = "Test Description",
                Location = "Test Location",
                Category = "Test Category"
            };

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventRepository.GetByIdAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEvents()
        {
            // Arrange
            var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Name = "Event 1", Description = "Description 1", Location = "Location 1", Category = "Category 1" },
            new Event { Id = Guid.NewGuid(), Name = "Event 2", Description = "Description 2", Location = "Location 2", Category = "Category 2" }
        };

            _context.Events.AddRange(events);
            await _context.SaveChangesAsync();

            // Act
            var result = await _eventRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetEventByPage_ReturnsPaginatedEvents()
        {
            // Arrange
            var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Name = "Event 1", Description = "Description 1", Location = "Location 1", Category = "Category 1" },
            new Event { Id = Guid.NewGuid(), Name = "Event 2", Description = "Description 2", Location = "Location 2", Category = "Category 2" },
            new Event { Id = Guid.NewGuid(), Name = "Event 3", Description = "Description 3", Location = "Location 3", Category = "Category 3" },
            new Event { Id = Guid.NewGuid(), Name = "Event 4", Description = "Description 4", Location = "Location 4", Category = "Category 4" },
            new Event { Id = Guid.NewGuid(), Name = "Event 5", Description = "Description 5", Location = "Location 5", Category = "Category 5" }
        };

            _context.Events.AddRange(events);
            await _context.SaveChangesAsync();

            int page = 1;
            int pageSize = 2;

            // Act
            var result = await _eventRepository.GetEventByPage(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageSize, result.Count());
        }

        [Fact]
        public void Query_ReturnsQueryableEvents()
        {
            // Arrange
            var events = new List<Event>
        {
            new Event { Id = Guid.NewGuid(), Name = "Event 1", Description = "Description 1", Location = "Location 1", Category = "Category 1" },
            new Event { Id = Guid.NewGuid(), Name = "Event 2", Description = "Description 2", Location = "Location 2", Category = "Category 2" }
        };

            _context.Events.AddRange(events);
            _context.SaveChanges();

            // Act
            var result = _eventRepository.Query();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
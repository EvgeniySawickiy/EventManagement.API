using EventManagement.Application.Services;
using EventManagement.Application.Use_Cases.EventUseCases;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EventManagement.Tests
{
    public class DeleteEventUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly DeleteEventUseCase _deleteEventUseCase;
        private readonly EventDbContext _context;
        private readonly Mock<IEventRepository> _eventRepositoryMock;

        public DeleteEventUseCaseTests()
        {
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new EventDbContext(options);
            _eventRepositoryMock = new Mock<IEventRepository>();

            _unitOfWork = new UnitOfWork(
                _context,
                new Mock<IUserRepository>().Object,
                new Mock<IParticipantRepository>().Object,
                _eventRepositoryMock.Object,
                new Mock<IEventParticipantRepository>().Object,
                new Mock<IImageRepository>().Object
            );

            _notificationService = new Mock<INotificationService>().Object;
            _deleteEventUseCase = new DeleteEventUseCase(_unitOfWork);
        }

        [Fact]
        public async Task DeleteEvent_Should_Remove_Event()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var existingEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = "New Event",
                Category = "Some Category",
                Description = "Event description",
                Location = "Event location", 
                EventDate = DateTime.UtcNow, 
            };
            _context.Events.Add(existingEvent);
            await _context.SaveChangesAsync();

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(existingEvent);
            _eventRepositoryMock.Setup(x => x.DeleteAsync(existingEvent)).Returns(Task.CompletedTask);

            // Act
            await _deleteEventUseCase.ExecuteAsync(eventId);

            // Assert
            _eventRepositoryMock.Verify(x => x.DeleteAsync(existingEvent), Times.Once);
            var result = await _context.Events.FindAsync(eventId);
            Assert.Null(result);
        }
    }
}

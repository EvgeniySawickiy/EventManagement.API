using AutoMapper;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using EventManagement.Core.Interfaces.Services;
using Moq;
using Xunit;

namespace EventManagement.Tests
{
    public class EventServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<INotificationService> _notificationerviceMock;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _notificationServiceMock = new Mock<INotificationService>();
            _eventService = new EventService(_unitOfWorkMock.Object, _mapperMock.Object, _notificationServiceMock.Object);
        }

        [Fact]
        public async Task GetAllEvents_ShouldReturnAllEvents()
        {
            // Arrange
            var eventList = new List<Event> { new Event { Name = "Event1" }, new Event { Name = "Event2" } };
            _unitOfWorkMock.Setup(uow => uow.Events.GetAllEventsAsync()).ReturnsAsync(eventList);

            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _unitOfWorkMock.Verify(uow => uow.Events.GetAllEventsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEventById_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventEntity = new Event { Id = eventId, Name = "Event1" };
            _unitOfWorkMock.Setup(uow => uow.Events.GetEventByIdAsync(eventId)).ReturnsAsync(eventEntity);

            // Act
            var result = await _eventService.GetEventByIdAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Event1", result.Name);
            _unitOfWorkMock.Verify(uow => uow.Events.GetEventByIdAsync(eventId), Times.Once);
        }

        [Fact]
        public async Task GetEventByName_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var eventName = "Event1";
            var eventList = new List<Event> { new Event { Name = eventName } };
            _unitOfWorkMock.Setup(uow => uow.Events.GetAllEventsAsync()).ReturnsAsync(eventList);

            // Act
            var result = await _eventService.GetEventByNameAsync(eventName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventName, result.Name);
            _unitOfWorkMock.Verify(uow => uow.Events.GetAllEventsAsync(), Times.Once);
        }


        [Fact]
        public async Task AddEvent_ShouldAddNewEvent()
        {
            // Arrange
            var newEvent = new Event { Name = "New Event" };
            _unitOfWorkMock.Setup(uow => uow.Events.AddEventAsync(newEvent)).Returns(Task.CompletedTask);

            // Act
            await _eventService.AddEventAsync(newEvent);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.Events.AddEventAsync(newEvent), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task UpdateEvent_ShouldUpdateEvent_WhenEventExists()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var existingEvent = new Event { Id = eventId, Name = "Old Event", EventDate = DateTime.Now, Location = "Old Location" };
            var updatedEvent = new Event { Id = eventId, Name = "Updated Event", EventDate = DateTime.Now.AddDays(1), Location = "New Location" };

            _unitOfWorkMock.Setup(uow => uow.Events.GetEventByIdAsync(eventId)).ReturnsAsync(existingEvent);
            _mapperMock.Setup(m => m.Map(updatedEvent, existingEvent));

            // Act
            await _eventService.UpdateEventAsync(eventId, updatedEvent);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.Events.UpdateEventAsync(existingEvent), Times.Once);
            _notificationServiceMock.Verify(ns => ns.NotifyParticipantsAsync(eventId, true, true), Times.Once); // Assuming both date and location changed
        }


        [Fact]
        public async Task DeleteEvent_ShouldDeleteEvent_WhenEventExists()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var eventEntity = new Event { Id = eventId };

            _unitOfWorkMock.Setup(uow => uow.Events.GetEventByIdAsync(eventId)).ReturnsAsync(eventEntity);
            _unitOfWorkMock.Setup(uow => uow.Events.DeleteEventAsync(eventId)).Returns(Task.CompletedTask);

            // Act
            await _eventService.DeleteEventAsync(eventId);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.Events.DeleteEventAsync(eventId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public void GetEventsByCriteria_ShouldReturnFilteredEvents()
        {
            // Arrange
            var eventList = new List<Event>
    {
        new Event { Name = "Event1", EventDate = DateTime.Now, Location = "Location1", Category = "Category1" },
        new Event { Name = "Event2", EventDate = DateTime.Now.AddDays(1), Location = "Location2", Category = "Category2" }
    }.AsQueryable();

            _unitOfWorkMock.Setup(uow => uow.Events.Query()).Returns(eventList);

            // Act
            var result = _eventService.GetEventsByCriteriaAsync(DateTime.Now, "Location1", "Category1");

            // Assert
            Assert.Single(result);
            Assert.Equal("Event1", result.First().Name);
        }

    }
}
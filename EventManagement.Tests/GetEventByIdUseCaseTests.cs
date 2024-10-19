using EventManagement.Application.Use_Cases.EventUseCases;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Tests
{
    public class GetEventByIdUseCaseTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly GetEventByIdUseCase _getEventByIdUseCase;

        public GetEventByIdUseCaseTests()
        {
            _unitOfWorkMock= new Mock<IUnitOfWork>();
            _eventRepositoryMock = new Mock<IEventRepository>();

            _unitOfWorkMock.Setup(u => u.Events).Returns(_eventRepositoryMock.Object);

            _getEventByIdUseCase = new GetEventByIdUseCase(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetEventById_Should_Return_Event_When_Id_Exists()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var @event = new Event { Id = eventId, Name = "Test Event" };
            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(@event);

            // Act
            var result = await _getEventByIdUseCase.ExecuteAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
            _eventRepositoryMock.Verify(x => x.GetByIdAsync(eventId), Times.Once);
        }
    }
}

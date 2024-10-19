using EventManagement.Application.Use_Cases.EventUseCases;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Moq;
using Xunit;

namespace EventManagement.Tests
{
    public class GetAllEventsUseCaseTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly GetAllEventsUseCase _getAllEventsUseCase;

        public GetAllEventsUseCaseTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _eventRepositoryMock = new Mock<IEventRepository>();

            _unitOfWorkMock.Setup(u => u.Events).Returns(_eventRepositoryMock.Object);

            _getAllEventsUseCase = new GetAllEventsUseCase(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetAllEvents_Should_Return_All_Events()
        {
            // Arrange
            var events = new List<Event> { new Event { Id = Guid.NewGuid(), Name = "Test Event" } };
            _eventRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(events);

            // Act
            var result = await _getAllEventsUseCase.ExecuteAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(events.Count, result.Count());
            _eventRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }
    }
}

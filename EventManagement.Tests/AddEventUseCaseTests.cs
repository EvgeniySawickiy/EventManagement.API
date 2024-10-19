using EventManagement.Application.Use_Cases.EventUseCases;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Moq;
using Xunit;

namespace EventManagement.Tests
{
    public class AddEventUseCaseTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly AddEventUseCase _addEventUseCase;

        public AddEventUseCaseTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _eventRepositoryMock = new Mock<IEventRepository>();

            _unitOfWorkMock.Setup(u => u.Events).Returns(_eventRepositoryMock.Object);

            _addEventUseCase = new AddEventUseCase(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task AddEvent_Should_Add_New_Event()
        {
            // Arrange
            var newEvent = new Event { Id = Guid.NewGuid(), Name = "Test Event", Location = "Test Location" };

            // Act
            await _addEventUseCase.ExecuteAsync(newEvent);

            // Assert
            _eventRepositoryMock.Verify(x => x.AddAsync(newEvent), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }

}

using EventManagement.Application.Use_Cases.ImageUseCases;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Moq;
using Xunit;

namespace EventManagement.Tests
{
    public class AddImageUseCaseTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IImageRepository> _imageRepositoryMock;
        private readonly AddImageUseCase _addImageUseCase;

        public AddImageUseCaseTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _imageRepositoryMock = new Mock<IImageRepository>();

            _unitOfWorkMock.Setup(u => u.Images).Returns(_imageRepositoryMock.Object);

            _addImageUseCase = new AddImageUseCase(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task AddImage_Should_Add_Image_To_Event()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var image = new Image { Id = Guid.NewGuid(), FilePath = "test.jpg", EventId = eventId };

            // Act
            await _addImageUseCase.ExecuteAsync(image,image.FilePath);

            // Assert
            _imageRepositoryMock.Verify(x => x.AddAsync(image), Times.Once);
        }
    }
}

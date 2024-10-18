using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;


namespace EventManagement.Application.Use_Cases.ImageUseCases
{
    public class AddImageUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddImageUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Image> ExecuteAsync(Image image)
        {
            await _unitOfWork.Images.AddAsync(image);
            return image;
        }
    }
}

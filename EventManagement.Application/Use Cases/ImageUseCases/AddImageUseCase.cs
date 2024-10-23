using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace EventManagement.Application.Use_Cases.ImageUseCases
{
    public class AddImageUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddImageUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Event> ExecuteAsync(Guid eventId, IFormFile imageFile)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine("wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var imageEntity = new Image
            {
                Id = Guid.NewGuid(),
                FilePath = filePath,
                EventId = eventId
            };
            await _unitOfWork.Images.AddAsync(imageEntity);
            var updatebleEvent = _unitOfWork.Events.GetByIdAsync(eventId);
            updatebleEvent.Result.ImageId = imageEntity.Id;
            return updatebleEvent.Result;
        }
    }
}

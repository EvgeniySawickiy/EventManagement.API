using EventManagement.Application.Exceptions;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;


namespace EventManagement.Application.Use_Cases.EventUseCases
{
    public class DeleteEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEventUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Guid eventId)
        {
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
            if (eventEntity == null)
            {
                throw new NotFoundException("Event not found");
            }

            await _unitOfWork.Events.DeleteAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

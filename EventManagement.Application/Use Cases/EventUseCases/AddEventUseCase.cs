using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Application.Use_Cases.EventUseCases
{
    public class AddEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddEventUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Event> ExecuteAsync(Event newEvent)
        {
            await _unitOfWork.Events.AddEventAsync(newEvent);
            await _unitOfWork.SaveChangesAsync();
            return newEvent;
        }
    }
}
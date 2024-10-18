using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Application.Use_Cases.EventUseCases
{
    public class GetEventByIdUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventByIdUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Event> ExecuteAsync(Guid eventId)
        {
            return await _unitOfWork.Events.GetByIdAsync(eventId);
        }
    }
}
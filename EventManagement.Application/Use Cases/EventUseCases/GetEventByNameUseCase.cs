using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Application.Use_Cases.EventUseCases
{
    public class GetEventByNameUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventByNameUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Event> ExecuteAsync(string eventName)
        {
            var events = await _unitOfWork.Events.GetAllAsync();
            return events.FirstOrDefault(e => e.Name == eventName);
        }
    }
}

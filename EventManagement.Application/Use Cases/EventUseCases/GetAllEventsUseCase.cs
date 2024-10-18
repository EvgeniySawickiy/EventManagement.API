using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;


namespace EventManagement.Application.Use_Cases.EventUseCases
{
    public class GetAllEventsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllEventsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Event>> ExecuteAsync()
        {
            return await _unitOfWork.Events.GetAllEventsAsync();
        }
    }
}
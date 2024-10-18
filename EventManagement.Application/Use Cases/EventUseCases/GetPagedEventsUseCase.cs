using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;


namespace EventManagement.Application.Use_Cases.EventUseCases
{
    public class GetPagedEventsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPagedEventsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Event>> ExecuteAsync(int pageNumber, int pageSize)
        {
            return await _unitOfWork.Events.GetEventByPage(pageNumber, pageSize);
        }
    }
}

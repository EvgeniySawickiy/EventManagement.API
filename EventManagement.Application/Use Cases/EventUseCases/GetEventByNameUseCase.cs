using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

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
            return await _unitOfWork.Events.Query().Where(e => e.Name == eventName).FirstOrDefaultAsync();
        }
    }
}

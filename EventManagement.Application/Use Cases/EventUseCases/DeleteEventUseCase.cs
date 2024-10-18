using EventManagement.Core.Interfaces.Repositories;


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
            await _unitOfWork.Events.DeleteEventAsync(eventId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Application.Use_Cases.ParticipantUseCases
{
    public class RemoveParticipantFromEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveParticipantFromEventUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Guid eventId, Guid userId)
        {
            var participant = _unitOfWork.Participants.GetAllParticipantsAsync().Result.First(p => p.UserId == userId);
            await _unitOfWork.EventParticipant.DeleteParticipantFromEventAsync(eventId, participant.Id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

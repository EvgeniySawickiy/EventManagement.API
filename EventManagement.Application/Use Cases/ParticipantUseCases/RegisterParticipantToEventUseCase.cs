using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Application.Use_Cases.ParticipantUseCases
{
    public class RegisterParticipantToEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterParticipantToEventUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Guid userId, Guid eventId)
        {
            var participant = _unitOfWork.Participants.GetAllAsync().Result.First(p => p.UserId == userId);
            var existingParticipant = _unitOfWork.EventParticipant.GetAllAsync()
                .Result.FirstOrDefault(ep => ep.ParticipantId == participant.Id && ep.EventId == eventId);

            if (existingParticipant != null)
            {
                throw new Exception("Participant is already registered for this event.");
            }

            var eventParticipant = new EventParticipant
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                ParticipantId = participant.Id,
                RegistrationDate = DateTime.UtcNow,
            };

            await _unitOfWork.EventParticipant.AddAsync(eventParticipant);
            await _unitOfWork.SaveChangesAsync();
        }
    }

}

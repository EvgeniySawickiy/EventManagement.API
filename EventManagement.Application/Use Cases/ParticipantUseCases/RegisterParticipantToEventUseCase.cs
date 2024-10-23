using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

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
            var participant = await _unitOfWork.Participants.Query().Where(p=>p.UserId == userId).FirstOrDefaultAsync();

            if (participant == null)
            {
                throw new Exception("Participant does not exist");
            }

            var existingParticipant = await _unitOfWork.EventParticipant.Query().Where(ep => ep.ParticipantId == participant.Id && ep.EventId == eventId).FirstOrDefaultAsync();

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

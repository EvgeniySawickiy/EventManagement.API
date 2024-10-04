using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using EventManagement.Core.Interfaces.Services;


namespace EventManagement.Application.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParticipantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Participant> GetParticipantByIdAsync(Guid id)
        {
            return await _unitOfWork.Participants.GetParticipantByIdAsync(id);
        }

        public async Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(Guid eventId)
        {
            List<EventParticipant> eventEntity = _unitOfWork.EventParticipant.GetAllParticipantsToEventAsync().Result.Where(e => e.EventId == eventId).ToList();
            var participantList = new List<Participant>();
            for (int i = 0; i < eventEntity.Count(); i++)
            {
                participantList.Add(_unitOfWork.Participants.GetParticipantByIdAsync(eventEntity[i].ParticipantId).Result);
            }
            return participantList;
        }

        public async Task RegisterParticipantToEventAsync(Guid userId, Guid eventId)
        {
            var participant = _unitOfWork.Participants.GetAllParticipantsAsync().Result.First(p => p.UserId == userId);

            var existingParticipant = _unitOfWork.EventParticipant.GetAllParticipantsToEventAsync().Result.FirstOrDefault(ep => ep.ParticipantId == participant.Id && ep.EventId == eventId);

            if (existingParticipant != null)
            {
                throw new Exception("Participant is already registered for this event.");
            }

            var eventParticipant = new EventParticipant
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                ParticipantId = participant.Id,
                RegistrationDate = DateTime.Now.ToUniversalTime(),
            };

            await _unitOfWork.EventParticipant.AddParticipantToEventAsync(eventParticipant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveParticipantFromEventAsync(Guid eventId, Guid userId)
        {
            var participant = _unitOfWork.Participants.GetAllParticipantsAsync().Result.First(p => p.UserId == userId);
            //var eventParticipant = _unitOfWork.EventParticipant.GetAllParticipantsToEventAsync().Result.First(e => e.ParticipantId == participant.Id && e.EventId == eventId);
            await _unitOfWork.EventParticipant.DeleteParticipantFromEventAsync(eventId, participant.Id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
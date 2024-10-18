using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Application.Use_Cases.ParticipantUseCases
{
    public class GetParticipantsByEventIdUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetParticipantsByEventIdUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Participant>> ExecuteAsync(Guid eventId)
        {
            List<EventParticipant> eventEntity = _unitOfWork.EventParticipant.GetAllParticipantsToEventAsync()
                .Result.Where(e => e.EventId == eventId).ToList();
            var participantList = new List<Participant>();
            for (int i = 0; i < eventEntity.Count; i++)
            {
                participantList.Add(_unitOfWork.Participants.GetParticipantByIdAsync(eventEntity[i].ParticipantId).Result);
            }
            return participantList;
        }
    }
}

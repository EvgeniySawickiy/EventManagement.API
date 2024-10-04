using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Repositories
{
    public interface IEventParticipantRepository
    {
        Task AddParticipantToEventAsync(EventParticipant eventParticipant);
        Task UpdateParticipantToEventAsync(EventParticipant eventParticipant);
        Task DeleteParticipantFromEventAsync(Guid eventId, Guid userId);
        Task<IEnumerable<EventParticipant>> GetAllParticipantsToEventAsync();

    }
}

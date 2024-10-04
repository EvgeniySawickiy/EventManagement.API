using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Services
{
    public interface IParticipantService
    {
        Task<Participant> GetParticipantByIdAsync(Guid id);
        Task<IEnumerable<Participant>> GetParticipantsByEventIdAsync(Guid eventId);
        Task RegisterParticipantToEventAsync(Guid userId, Guid eventId);
        Task RemoveParticipantFromEventAsync(Guid eventId, Guid userId);
    }
}

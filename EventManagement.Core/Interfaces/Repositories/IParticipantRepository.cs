using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Repositories
{
    public interface IParticipantRepository
    {
        Task<IEnumerable<Participant>> GetAllParticipantsAsync();
        Task<Participant> GetParticipantByIdAsync(Guid id);
        Task AddParticipantAsync(Participant participant);
        Task UpdateParticipantAsync(Participant participant);
        Task DeleteParticipantAsync(Guid id);
    }
}
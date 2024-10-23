using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Repositories
{
    public interface IEventParticipantRepository :IRepository<EventParticipant>
    {
        IQueryable<EventParticipant> Query();
    }
}

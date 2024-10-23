using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Repositories
{
    public interface IParticipantRepository : IRepository<Participant>
    {
        IQueryable<Participant> Query();
    }
}
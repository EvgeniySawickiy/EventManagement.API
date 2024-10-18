using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.DataAccess.Repositories
{
    public class EventParticipantRepository :RepositoryBase<EventParticipant>, IEventParticipantRepository
    {
        private readonly EventDbContext _context;

        public EventParticipantRepository(EventDbContext context):base(context) 
        {
            _context = context;
        }

        public new async Task<EventParticipant> GetByIdAsync(Guid id)
        {
            return await _context.EventParticipants
                .Include(e=>e.Participant)
                .Include(e=>e.Event)
                .FirstOrDefaultAsync(e=>e.Id == id);
        }

        public new async Task<IEnumerable<EventParticipant>> GetAllAsync()
        {
            return await _context.EventParticipants
                .Include(e => e.Participant)
                .Include(e => e.Event)
                .ToListAsync();
        }
    }
}

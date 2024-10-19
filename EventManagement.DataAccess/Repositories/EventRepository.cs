using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.DataAccess.Repositories
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        private readonly EventDbContext _context;

        public EventRepository(EventDbContext context) : base(context)
        {
            _context = context;
        }

        public new async Task<Event> GetByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.EventParticipants)
                .Include(e => e.Image)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public new async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.EventParticipants)
                .Include(e => e.Image)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventByPage(int page, int pageSize)
        {
            return await _context.Events
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public IQueryable<Event> Query()
        {
            return _context.Events.AsQueryable();
        }
    }
}
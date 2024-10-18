using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;


namespace EventManagement.DataAccess.Repositories
{
    public class ParticipantRepository :RepositoryBase<Participant>, IParticipantRepository
    {
        private readonly EventDbContext _context;

        public ParticipantRepository(EventDbContext context) :base(context)
        {
            _context = context;
        }

        public new async Task<IEnumerable<Participant>> GetAllAsync()
        {
            return await _context.Participants
                .Include(p=>p.EventParticipants)
                .ToListAsync();
        }

        public new async Task<Participant> GetByIdAsync(Guid id)
        {
            return await _context.Participants
                .Include(p => p.EventParticipants)
                .FirstOrDefaultAsync(p=>p.Id==id);
        }
    }
}
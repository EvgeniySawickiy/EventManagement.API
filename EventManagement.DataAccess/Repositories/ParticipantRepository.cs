using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;


namespace EventManagement.DataAccess.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly EventDbContext _context;

        public ParticipantRepository(EventDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Participant>> GetAllParticipantsAsync()
        {
            return await _context.Participants.ToListAsync();
        }

        public async Task<Participant> GetParticipantByIdAsync(Guid id)
        {
            return await _context.Participants.FindAsync(id);
        }

        public async Task AddParticipantAsync(Participant participant)
        {
            await _context.Participants.AddAsync(participant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateParticipantAsync(Participant participant)
        {
            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteParticipantAsync(Guid id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant != null)
            {
                _context.Participants.Remove(participant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
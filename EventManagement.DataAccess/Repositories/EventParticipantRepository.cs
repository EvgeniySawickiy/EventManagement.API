using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.DataAccess.Repositories
{
    public class EventParticipantRepository : IEventParticipantRepository
    {
        private readonly EventDbContext _context;

        public EventParticipantRepository(EventDbContext context)
        {
            _context = context;
        }

        public async Task AddParticipantToEventAsync(EventParticipant eventParticipant)
        {
            await _context.EventParticipants.AddAsync(eventParticipant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateParticipantToEventAsync(EventParticipant eventParticipant)
        {
            _context.EventParticipants.Update(eventParticipant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteParticipantFromEventAsync(Guid eventId,Guid participantId)
        {
            var eventParticipantEntity = await _context.EventParticipants.FindAsync(eventId, participantId);
            if (eventParticipantEntity != null)
            {
                _context.EventParticipants.Remove(eventParticipantEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<EventParticipant>> GetAllParticipantsToEventAsync()
        {
           return await _context.EventParticipants.ToListAsync();
        }
    }
}

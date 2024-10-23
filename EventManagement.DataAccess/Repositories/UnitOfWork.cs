using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EventDbContext _context;

        public IUserRepository Users { get; }
        public IParticipantRepository Participants { get; }
        public IEventRepository Events { get; }
        public IEventParticipantRepository EventParticipant { get; }
        public IImageRepository Images { get; }

        public UnitOfWork(EventDbContext context, IUserRepository userRepository, IParticipantRepository participantRepository, IEventRepository eventRepository,IEventParticipantRepository eventParticipant, IImageRepository images)
        {
            _context = context;
            Users = userRepository;
            Participants = participantRepository;
            Events = eventRepository;
            EventParticipant = eventParticipant;
            Images = images;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
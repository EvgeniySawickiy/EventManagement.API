namespace EventManagement.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IParticipantRepository Participants { get; }
        IEventRepository Events { get; }
        IEventParticipantRepository EventParticipant { get; }
        IImageRepository Images { get; }
        Task<int> SaveChangesAsync();
    }
}
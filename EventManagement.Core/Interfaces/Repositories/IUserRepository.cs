using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Repositories
{
    public interface IUserRepository :IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}
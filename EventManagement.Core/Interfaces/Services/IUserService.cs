
using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByUsernameAsync(string username);
        Task<bool> RegisterUserAsync(User newUser, Participant newParticipant);
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }
}

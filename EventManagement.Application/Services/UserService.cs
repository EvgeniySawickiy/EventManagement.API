using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using EventManagement.Core.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace EventManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _unitOfWork.Users.GetByUsernameAsync(username);
        }

        public async Task<bool> RegisterUserAsync(User newUser, Participant newParticipant)
        {
            var passwordHash = HashPassword(newUser.PasswordHash);

            var user = new User
            {
                Id= Guid.NewGuid(),
                Username = newUser.Username,
                Email = newUser.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                Role= "User"
            };

            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                FirstName = newParticipant.FirstName,
                LastName = newParticipant.LastName,
                BirthDate= newParticipant.BirthDate,
                Email = newParticipant.Email,
                UserId= user.Id,

            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.Participants.AddParticipantAsync(participant);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null)
                return false;

            var passwordHash = HashPassword(password);
            return user.PasswordHash == passwordHash;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
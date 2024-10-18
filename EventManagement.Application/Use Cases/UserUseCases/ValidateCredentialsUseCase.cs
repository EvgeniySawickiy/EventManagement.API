using EventManagement.Core.Interfaces.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace EventManagement.Application.Use_Cases.UserUseCases
{
    public class ValidateCredentialsUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidateCredentialsUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ExecuteAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null)
                throw new Exception("Invalid username or password");


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

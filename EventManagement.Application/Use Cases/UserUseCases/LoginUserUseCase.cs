using EventManagement.Application.Interfaces;
using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Application.Use_Cases.UserUseCases
{
    public class LoginUserUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidateCredentialsUseCase _validateCredentialsUseCase;
        private readonly GetUserByUsernameUseCase _getUserByUsernameUseCase;
        private readonly IJwtService _jwtService;


        public LoginUserUseCase(IUnitOfWork unitOfWork, ValidateCredentialsUseCase validateCredentialsUseCase, GetUserByUsernameUseCase getUserByUsernameUseCase, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _getUserByUsernameUseCase = getUserByUsernameUseCase;
            _jwtService = jwtService;
            _validateCredentialsUseCase = validateCredentialsUseCase;
        }

        public async Task<string> ExecuteAsync(string username, string password)
        {
           var IsValid = await _validateCredentialsUseCase.ExecuteAsync(username, password);
            if (IsValid)
            {
                var user = await _getUserByUsernameUseCase.ExecuteAsync(username);

                return _jwtService.GenerateToken(user.Username, user.Role);
            }
            else
            {
                throw new Exception("Invalid username or password");
            }
        }
    }
}

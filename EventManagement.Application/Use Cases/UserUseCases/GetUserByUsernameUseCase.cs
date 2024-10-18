using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Application.Use_Cases.UserUseCases
{
    public class GetUserByUsernameUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserByUsernameUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> ExecuteAsync(string username)
        {
            return await _unitOfWork.Users.GetByUsernameAsync(username);
        }
    }
}

using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;


namespace EventManagement.DataAccess.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly EventDbContext _context;

        public UserRepository(EventDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Username == username);
        }

    }
}
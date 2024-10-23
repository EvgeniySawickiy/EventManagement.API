using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Infrastructure.Repositories
{
    public class ImageRepository :RepositoryBase<Image>,IImageRepository
    {
        private readonly EventDbContext _context;
        public ImageRepository(EventDbContext context) :base(context)
        {
            _context = context;
        }

        public new async Task<Image> GetByIdAsync(Guid id)
        {
            return await _context.Images
                .Include(i=>i.Event)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}

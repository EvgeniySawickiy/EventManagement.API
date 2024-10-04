using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.DataAccess.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly EventDbContext _context;
        public ImageRepository(EventDbContext context)
        {
            _context = context;
        }
        public async Task AddImageAsync(Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(Guid id)
        {
            var imageEntity = await _context.Images.FindAsync(id);
            if (imageEntity != null)
            {
                _context.Images.Remove(imageEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Image> GetImageByIdAsync(Guid id)
        {
            return await _context.Images.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task UpdateImageAsync(Image image)
        {
            _context.Images.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Image>> GetAllImagesToEventAsync()
        {
            return await _context.Images.ToListAsync();
        }
    }
}

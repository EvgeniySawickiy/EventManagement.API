
using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Repositories
{
    public interface IImageRepository
    {
        Task<IEnumerable<Image>> GetAllImagesToEventAsync();
        Task<Image> GetImageByIdAsync(Guid id);
        Task AddImageAsync(Image image);
        Task UpdateImageAsync(Image image);
        Task DeleteImageAsync(Guid id);
    }
}

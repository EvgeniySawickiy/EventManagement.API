using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Services
{
    public interface IImageService
    {
        Task AddImageAsync(Image image);
    }
}

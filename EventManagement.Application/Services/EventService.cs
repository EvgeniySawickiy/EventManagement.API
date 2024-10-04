using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using EventManagement.Core.Interfaces.Services;

namespace EventManagement.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Event> GetEventByIdAsync(Guid id)
        {
            return await _unitOfWork.Events.GetEventByIdAsync(id);
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _unitOfWork.Events.GetAllEventsAsync();
        }

        public async Task AddEventAsync(Event eventEntity)
        {
            await _unitOfWork.Events.AddEventAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateEventAsync(Guid eventId, Event eventEntity)
        {
            eventEntity.Id = eventId;
            await _unitOfWork.Events.UpdateEventAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(Guid id)
        {
            await _unitOfWork.Events.DeleteEventAsync(id);
            var eventImage = _unitOfWork.Images.GetAllImagesToEventAsync().Result.First(i => i.EventId == id);
            if (eventImage != null)
            {
                await _unitOfWork.Images.DeleteImageAsync(eventImage.Id);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Event> GetEventByNameAsync(string eventName)
        {
            return _unitOfWork.Events.GetAllEventsAsync().Result.First(e => e.Name == eventName);
        }

        public IEnumerable<Event> GetEventsByCriteriaAsync(DateTime? date, string location, string category)
        {
            var query = _unitOfWork.Events.Query();

            if (date.HasValue)
            {
                query = query.Where(e => e.EventDate.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(e => e.Location.Contains(location));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category.ToLower() == category.ToLower());
            }

            return query.ToList();
        }

    }
}
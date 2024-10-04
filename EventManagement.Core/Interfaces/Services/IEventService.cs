
using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Services
{
    public interface IEventService
    {
        Task<Event> GetEventByIdAsync(Guid id);
        Task<Event> GetEventByNameAsync(string eventName);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task AddEventAsync(Event eventEntity);
        Task UpdateEventAsync(Guid eventId, Event eventEntity);
        Task DeleteEventAsync(Guid id);
        IEnumerable<Event> GetEventsByCriteriaAsync(DateTime? date, string location, string category);
    }
}

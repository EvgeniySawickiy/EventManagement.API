﻿
using EventManagement.Core.Entity;

namespace EventManagement.Core.Interfaces.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(Guid id);
        Task AddEventAsync(Event eventEntity);
        Task UpdateEventAsync(Event eventEntity);
        Task DeleteEventAsync(Guid id);
        Task<List<Event>> GetEventByPage(int page, int pageSize);
        IQueryable<Event> Query();
    }
}

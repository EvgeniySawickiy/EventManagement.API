using AutoMapper;
using EventManagement.Application.Exceptions;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using EventManagement.Core.Interfaces.Services;
using EventManagement.Application.Services;
using EventManagement.Application.DTO.Request;

namespace EventManagement.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public EventService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
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
            var eventEnt = await _unitOfWork.Events.GetEventByIdAsync(eventId);
            if (eventEnt == null)
            {
                throw new NotFoundException("Event not found");
            }

            bool isDateChanged = eventEnt.EventDate != eventEntity.EventDate;
            bool isLocationChanged = eventEnt.Location != eventEntity.Location;

            _mapper.Map(eventEntity, eventEnt);
            await _unitOfWork.Events.UpdateEventAsync(eventEnt);

            if (isDateChanged || isLocationChanged)
            {
                await _notificationService.NotifyParticipantsAsync(eventEnt.Id, isDateChanged, isLocationChanged);
            }
        }

        public async Task DeleteEventAsync(Guid id)
        {
            await _unitOfWork.Events.DeleteEventAsync(id);
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

        public async Task<List<Event>> GetPagedEventsAsync(int pageNumber, int pageSize)
        {
            return await _unitOfWork.Events.GetEventByPage(pageNumber,pageSize);
        }
    }
}
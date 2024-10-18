using AutoMapper;
using EventManagement.Application.DTO.Request;
using EventManagement.Application.DTO.Response;
using EventManagement.Application.Use_Cases.EventUseCases;
using EventManagement.Application.Use_Cases.ImageUseCases;
using EventManagement.Core.Entity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IValidator<EventRequestDTO> _validator;
        private readonly AddImageUseCase _addImageUseCase;
        private readonly GetAllEventsUseCase _getAllEventsUseCase;
        private readonly GetEventByIdUseCase _getEventByIdUseCase;
        private readonly GetEventByNameUseCase _getEventByNameUseCase;
        private readonly AddEventUseCase _addEventUseCase;
        private readonly UpdateEventUseCase _updateEventUseCase;
        private readonly DeleteEventUseCase _deleteEventUseCase;
        private readonly GetEventsByCriteriaUseCase _getEventsByCriteriaUseCase;
        private readonly GetPagedEventsUseCase _getPagedEventsUseCase;

        public EventController(
       IMapper mapper,
       AddImageUseCase addImageUseCase,
       IValidator<EventRequestDTO> validator,
       GetAllEventsUseCase getAllEventsUseCase,
       GetEventByIdUseCase getEventByIdUseCase,
       GetEventByNameUseCase getEventByNameUseCase,
       AddEventUseCase addEventUseCase,
       UpdateEventUseCase updateEventUseCase,
       DeleteEventUseCase deleteEventUseCase,
       GetEventsByCriteriaUseCase getEventsByCriteriaUseCase,
       GetPagedEventsUseCase getPagedEventsUseCase)
        {
            _mapper = mapper;
            _addImageUseCase = addImageUseCase;
            _validator = validator;
            _getAllEventsUseCase = getAllEventsUseCase;
            _getEventByIdUseCase = getEventByIdUseCase;
            _getEventByNameUseCase = getEventByNameUseCase;
            _addEventUseCase = addEventUseCase;
            _updateEventUseCase = updateEventUseCase;
            _deleteEventUseCase = deleteEventUseCase;
            _getEventsByCriteriaUseCase = getEventsByCriteriaUseCase;
            _getPagedEventsUseCase = getPagedEventsUseCase;
        }

        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _getAllEventsUseCase.ExecuteAsync();
            var eventDtos = _mapper.Map<IEnumerable<EventResponseDTO>>(events);
            return Ok(eventDtos);
        }

        [HttpGet("{eventId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetEventsById(Guid eventId)
        {
            var events = await _getEventByIdUseCase.ExecuteAsync(eventId);
            if (events == null)
                return NotFound("Event not found");

            var eventDto = _mapper.Map<EventResponseDTO>(events);
            return Ok(eventDto);
        }

        [HttpGet("byname/{eventName}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetEventsByName(string eventName)
        {
            var events = await _getEventByNameUseCase.ExecuteAsync(eventName);
            if (events == null)
                return NotFound("Event not found");

            var eventDto = _mapper.Map<EventResponseDTO>(events);
            return Ok(eventDto);
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AddEvent([FromBody] EventRequestDTO model)
        {
            var validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            var eventEntity = _mapper.Map<Event>(model);
            await _addEventUseCase.ExecuteAsync(eventEntity);
            return Ok("Event added successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventRequestDTO model)
        {
            var validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            var eventEntity = _mapper.Map<Event>(model);
            await _updateEventUseCase.ExecuteAsync(id, eventEntity);
            return Ok("Event updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            await _deleteEventUseCase.ExecuteAsync(id);
            return Ok("Event deleted successfully");
        }

        [HttpGet("filter")]
        public IActionResult GetEventsByCriteria(DateTime? date, string? location, string? category)
        {
            var events = _getEventsByCriteriaUseCase.ExecuteAsync(date, location, category);
            return Ok(events);
        }

        [HttpPost("{eventId}/upload-image")]
        public async Task<IActionResult> UploadImage(Guid eventId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var eventEntity = await _getEventByIdUseCase.ExecuteAsync(eventId);
            if (eventEntity == null)
            {
                return NotFound("Event not found.");
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine("wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var imageEntity = new Image
            {
                Id = Guid.NewGuid(),
                FilePath = filePath,
                EventId = eventId
            };
            await _addImageUseCase.ExecuteAsync(imageEntity);

            eventEntity.ImageId = imageEntity.Id;
            await _updateEventUseCase.ExecuteAsync(eventId, eventEntity);

            return Ok(new { Message = "Image uploaded successfully.", ImagePath = filePath });
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedEvents([FromQuery] PaginationParams paginationParams)
        {
            var pagedEvents = await _getPagedEventsUseCase.ExecuteAsync(paginationParams.PageNumber, paginationParams.PageSize);
            return Ok(pagedEvents);
        }
    }
}
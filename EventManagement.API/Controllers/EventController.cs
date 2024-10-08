﻿using AutoMapper;
using EventManagement.Application.DTO.Request;
using EventManagement.Application.DTO.Response;
using EventManagement.Application.Validation;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IValidator<EventRequestDTO> _validator;

        public EventController(IEventService eventService, IMapper mapper, IImageService imageService, IValidator<EventRequestDTO> validator)
        {
            _eventService = eventService;
            _mapper = mapper;
            _imageService = imageService;
            _validator = validator;
        }

        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            var eventDtos = _mapper.Map<IEnumerable<EventResponseDTO>>(events);
            return Ok(eventDtos);
        }
        [HttpGet("{eventId}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetEventsById(Guid eventId)
        {
            var events = await _eventService.GetEventByIdAsync(eventId);
            if (events == null)
                return NotFound("User not found");

            var eventDto = _mapper.Map<EventResponseDTO>(events);
            return Ok(eventDto);
        }
        [HttpGet("byname/{eventName}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetEventsByName(string eventName)
        {
            var events = await _eventService.GetEventByNameAsync(eventName);
            if (events == null)
                return NotFound("User not found");

            var eventDto = _mapper.Map<EventResponseDTO>(events);
            return Ok(eventDto);
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AddEvent([FromBody] EventRequestDTO model)
        {
            var validationResult = await _validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
                var eventEntity = _mapper.Map<Event>(model);
                await _eventService.AddEventAsync(eventEntity);
                return Ok("Event added successfully");
            }
            else
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventRequestDTO model)
        {
            var validationResult = await _validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
            var eventEntity = _mapper.Map<Event>(model);
            await _eventService.UpdateEventAsync(id, eventEntity);
            return Ok("Event updated successfully");
            }
            else
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            await _eventService.DeleteEventAsync(id);
            return Ok("Event deleted successfully");
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetEventsByCriteria(DateTime? date, string? location, string? category)
        {
            var events = _eventService.GetEventsByCriteriaAsync(date, location, category);
            return Ok(events);
        }

        [HttpPost("{eventId}/upload-image")]
        public async Task<IActionResult> UploadImage(Guid eventId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var eventEntity = await _eventService.GetEventByIdAsync(eventId);
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
            await _imageService.AddImageAsync(imageEntity);

            eventEntity.ImageId = imageEntity.Id;
            await _eventService.UpdateEventAsync(eventId, eventEntity);

            return Ok(new { Message = "Image uploaded successfully.", ImagePath = filePath });
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedEvents([FromQuery] PaginationParams paginationParams)
        {
            var pagedEvents = await _eventService.GetPagedEventsAsync(paginationParams.PageNumber, paginationParams.PageSize);
            return Ok(pagedEvents);
        }
    }
}
using AutoMapper;
using EventManagement.Application.DTO.Request;
using EventManagement.Application.DTO.Response;
using EventManagement.Core.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantService _participantService;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterParticipantToEventRequestDTO> _validator;


        public ParticipantController(IParticipantService participantService, IMapper mapper, IValidator<RegisterParticipantToEventRequestDTO> validator)
        {
            _participantService = participantService;
            _mapper = mapper;
            _validator = validator;
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterParticipant([FromBody] RegisterParticipantToEventRequestDTO model)
        {
            var validationResult = await _validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {

                await _participantService.RegisterParticipantToEventAsync(model.UserId, model.EventId);
                return Ok("Participant registered successfully");
            }
            else
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetParticipantsByEvent(Guid eventId)
        {
            var participants = await _participantService.GetParticipantsByEventIdAsync(eventId);
            var participantsDto = _mapper.Map<IEnumerable<ParticipantResponseDTO>>(participants);
            return Ok(participantsDto);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("cancel")]
        public async Task<IActionResult> RemoveParticipantFromEvent(Guid eventId, Guid userId)
        {
            await _participantService.RemoveParticipantFromEventAsync(eventId, userId);
            return Ok("Participant removed successfully");
        }
    }
}
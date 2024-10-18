using AutoMapper;
using EventManagement.Application.DTO.Request;
using EventManagement.Application.DTO.Response;
using EventManagement.Application.Use_Cases.ParticipantUseCases;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController : ControllerBase
    {
        private readonly RegisterParticipantToEventUseCase _registerParticipantToEventUseCase;
        private readonly GetParticipantsByEventIdUseCase _getParticipantsByEventUseCase;
        private readonly RemoveParticipantFromEventUseCase _removeParticipantFromEventUseCase;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterParticipantToEventRequestDTO> _validator;

        public ParticipantController(
         RegisterParticipantToEventUseCase registerParticipantToEventUseCase,
         GetParticipantsByEventIdUseCase getParticipantsByEventUseCase,
         RemoveParticipantFromEventUseCase removeParticipantFromEventUseCase,
         IMapper mapper,
         IValidator<RegisterParticipantToEventRequestDTO> validator)
        {
            _registerParticipantToEventUseCase = registerParticipantToEventUseCase;
            _getParticipantsByEventUseCase = getParticipantsByEventUseCase;
            _removeParticipantFromEventUseCase = removeParticipantFromEventUseCase;
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
                await _registerParticipantToEventUseCase.ExecuteAsync(model.UserId, model.EventId);
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
            var participants = await _getParticipantsByEventUseCase.ExecuteAsync(eventId);
            var participantsDto = _mapper.Map<IEnumerable<ParticipantResponseDTO>>(participants);
            return Ok(participantsDto);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("cancel")]
        public async Task<IActionResult> RemoveParticipantFromEvent(Guid eventId, Guid userId)
        {
            await _removeParticipantFromEventUseCase.ExecuteAsync(eventId, userId);
            return Ok("Participant removed successfully");
        }
    }
}
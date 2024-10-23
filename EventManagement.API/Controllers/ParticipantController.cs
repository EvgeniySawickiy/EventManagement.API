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

        public ParticipantController(
         RegisterParticipantToEventUseCase registerParticipantToEventUseCase,
         GetParticipantsByEventIdUseCase getParticipantsByEventUseCase,
         RemoveParticipantFromEventUseCase removeParticipantFromEventUseCase,
         IMapper mapper)
        {
            _registerParticipantToEventUseCase = registerParticipantToEventUseCase;
            _getParticipantsByEventUseCase = getParticipantsByEventUseCase;
            _removeParticipantFromEventUseCase = removeParticipantFromEventUseCase;
            _mapper = mapper;
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterParticipant([FromBody] RegisterParticipantToEventRequestDTO model)
        {
            await _registerParticipantToEventUseCase.ExecuteAsync(model.UserId, model.EventId);

            return Ok("Participant registered successfully");
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetParticipantsByEvent(Guid eventId)
        {
            var participants = await _getParticipantsByEventUseCase.ExecuteAsync(eventId);
            var participantsDto = _mapper.Map<IEnumerable<ParticipantResponseDTO>>(participants);

            return Ok(participantsDto);
        }

        [HttpDelete("cancel")]
        public async Task<IActionResult> RemoveParticipantFromEvent(Guid eventId, Guid userId)
        {
            await _removeParticipantFromEventUseCase.ExecuteAsync(eventId, userId);

            return Ok("Participant removed successfully");
        }
    }
}
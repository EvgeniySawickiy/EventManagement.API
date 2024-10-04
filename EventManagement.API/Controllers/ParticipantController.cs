﻿using AutoMapper;
using EventManagement.Application.DTO.Request;
using EventManagement.Application.DTO.Response;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Services;
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

        public ParticipantController(IParticipantService participantService, IMapper mapper)
        {
            _participantService = participantService;
            _mapper = mapper;
        }

        [Authorize(Policy = "UserPolicy")] // Доступ для пользователей и администраторов
        [HttpPost("register")]
        public async Task<IActionResult> RegisterParticipant([FromBody] RegisterParticipantToEventRequestDTO model)
        {
            var participant = _mapper.Map<Participant>(model);
            await _participantService.RegisterParticipantToEventAsync(model.UserId,model.EventId);
            return Ok("Participant registered successfully");
        }

        [Authorize(Policy = "UserPolicy")] // Доступ для пользователей и администраторов
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetParticipantsByEvent(Guid eventId)
        {
            var participants = await _participantService.GetParticipantsByEventIdAsync(eventId);
            var participantsDto = _mapper.Map<IEnumerable<ParticipantResponseDTO>>(participants);
            return Ok(participantsDto);
        }

        [Authorize(Policy = "AdminPolicy")] // Только администратор может удалять участников
        [HttpDelete("cancel")]
        public async Task<IActionResult> RemoveParticipantFromEvent(Guid eventId,Guid userId)
        {
            await _participantService.RemoveParticipantFromEventAsync(eventId,userId);
            return Ok("Participant removed successfully");
        }
    }
}
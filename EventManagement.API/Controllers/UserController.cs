using AutoMapper;
using EventManagement.Application.DTO.Request;
using EventManagement.Application.DTO.Response;
using EventManagement.Application.Services;
using EventManagement.Application.Validation;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IValidator<RegisterRequestDTO> _RegisterValidator;

        public UserController(IUserService userService, IMapper mapper, JwtService jwtService, IValidator<RegisterRequestDTO> registerValidator)
        {
            _userService = userService;
            _mapper = mapper;
            _jwtService = jwtService;
            _RegisterValidator = registerValidator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerModel)
        {
            var validationResult = await _RegisterValidator.ValidateAsync(registerModel);
            if (validationResult.IsValid)
            {
                var user = _mapper.Map<User>(registerModel.UserModel);
                var participant = _mapper.Map<Participant>(registerModel.ParticipantModel);
                var result = await _userService.RegisterUserAsync(user, participant);
                if (!result)
                    return BadRequest("User with this username already exists");

                return Ok("User registered successfully");
            }
            else 
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserRequestDTO model)
        {
            var isValid = await _userService.ValidateCredentialsAsync(model.Username, model.Password);
            if (!isValid)
                return Unauthorized("Invalid username or password");

            var user = await _userService.GetByUsernameAsync(model.Username);
            var token = _jwtService.GenerateToken(user.Username, user.Role);

            return Ok(new { Token = token });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "UserPolicy")] 
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            var userDto = _mapper.Map<UserResponseDTO>(user);
            return Ok(userDto);
        }
    }
}
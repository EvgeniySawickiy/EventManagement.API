using AutoMapper;
using EventManagement.API.Services;
using EventManagement.Application.DTO.Request;
using EventManagement.Application.DTO.Response;
using EventManagement.Application.Use_Cases.UserUseCases;
using EventManagement.Core.Entity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly ValidateCredentialsUseCase _validateCredentialsUseCase;
        private readonly GetUserByIdUseCase _getUserByIdUseCase;
        private readonly GetUserByUsernameUseCase _getUserByUsernameUseCase;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IValidator<RegisterRequestDTO> _registerValidator;

        public UserController(
                RegisterUserUseCase registerUserUseCase,
                ValidateCredentialsUseCase validateCredentialsUseCase,
                GetUserByIdUseCase getUserByIdUseCase,
                IMapper mapper,
                JwtService jwtService,
                IValidator<RegisterRequestDTO> registerValidator,
                GetUserByUsernameUseCase getUserByUsernameUseCase)
        {
            _registerUserUseCase = registerUserUseCase;
            _validateCredentialsUseCase = validateCredentialsUseCase;
            _getUserByIdUseCase = getUserByIdUseCase;
            _getUserByUsernameUseCase = getUserByUsernameUseCase;
            _mapper = mapper;
            _jwtService = jwtService;
            _registerValidator = registerValidator;
        }

        [HttpPost("register")]  
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerModel)
        {
            var validationResult = await _registerValidator.ValidateAsync(registerModel);

            var user = _mapper.Map<User>(registerModel.UserModel);
            var participant = _mapper.Map<Participant>(registerModel.ParticipantModel);

            var result = await _registerUserUseCase.ExecuteAsync(user, participant);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserRequestDTO model)
        {
            var isValid = await _validateCredentialsUseCase.ExecuteAsync(model.Username, model.Password);

            var user = await _getUserByUsernameUseCase.ExecuteAsync(model.Username);
            var token = _jwtService.GenerateToken(user.Username, user.Role);

            return Ok(new { Token = token });
        }
        [HttpGet("{id}")]
        [Authorize(Policy = "UserPolicy")]

        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _getUserByIdUseCase.ExecuteAsync(id);

            var userDto = _mapper.Map<UserResponseDTO>(user);
            return Ok(userDto);
        }
    }
}
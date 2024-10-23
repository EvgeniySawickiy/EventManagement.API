using AutoMapper;
using EventManagement.Application.DTO.Request;
using EventManagement.Application.DTO.Response;
using EventManagement.Application.Use_Cases.UserUseCases;
using EventManagement.Core.Entity;
using EventManagement.Infrastructure.Security;
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
        private readonly LoginUserUseCase _loginUserUseCase;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;

        public UserController(
                RegisterUserUseCase registerUserUseCase,
                ValidateCredentialsUseCase validateCredentialsUseCase,
                GetUserByIdUseCase getUserByIdUseCase,
                IMapper mapper,
                JwtService jwtService,
                GetUserByUsernameUseCase getUserByUsernameUseCase,
                LoginUserUseCase loginUserUseCase)
        {
            _registerUserUseCase = registerUserUseCase;
            _validateCredentialsUseCase = validateCredentialsUseCase;
            _getUserByIdUseCase = getUserByIdUseCase;
            _getUserByUsernameUseCase = getUserByUsernameUseCase;
            _mapper = mapper;
            _jwtService = jwtService;
            _loginUserUseCase = loginUserUseCase;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerModel)
        {
            var user = _mapper.Map<User>(registerModel.UserModel);
            var participant = _mapper.Map<Participant>(registerModel.ParticipantModel);

            var result = await _registerUserUseCase.ExecuteAsync(user, participant);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserRequestDTO model)
        {
            var token =  await _loginUserUseCase.ExecuteAsync(model.Username, model.Password);

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
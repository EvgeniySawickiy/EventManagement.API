using EventManagement.Application.DTO.Request;
using EventManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EventManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO tokenRequest)
        {
            try
            {
                var (newAccessToken, newRefreshToken) = await _jwtService.GetRefreshToken(tokenRequest);
                return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Invalid refresh token");
            }
        }
    }
}

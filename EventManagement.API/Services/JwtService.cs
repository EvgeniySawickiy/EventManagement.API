using EventManagement.Application.DTO.Request;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EventManagement.API.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public JwtService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public string GenerateToken(string username, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Role, role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task SaveRefreshToken(string username, string refreshToken)
        {
            var token = new RefreshToken
            {
                Token = refreshToken,
                Username = username,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _refreshTokenRepository.SaveRefreshTokenAsync(token);
        }

        public async Task<bool> ValidateRefreshToken(string username, string refreshToken)
        {
            var token = await _refreshTokenRepository.GetRefreshTokenAsync(username, refreshToken);

            if (token == null || token.ExpiryDate <= DateTime.UtcNow || token.IsRevoked)
            {
                return false;
            }

            return true;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, 
                ValidateIssuer = false,  
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                ValidateLifetime = false 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<(string newAccessToken, string newRefreshToken)> GetRefreshToken(TokenRequestDTO tokenRequest)
        {
            var principal = GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
            var username = principal.Identity.Name;

            if (!await ValidateRefreshToken(username, tokenRequest.RefreshToken))
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newAccessToken = GenerateToken(username, principal.FindFirst(ClaimTypes.Role).Value);

            var newRefreshToken = Guid.NewGuid().ToString();
            await SaveRefreshToken(username, newRefreshToken);

            return (newAccessToken, newRefreshToken);
        }
    }
}

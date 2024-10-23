using EventManagement.Application.DTO.Request;
using System.Security.Claims;

namespace EventManagement.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string username, string role);
        Task<(string newAccessToken, string newRefreshToken)> GetRefreshToken(TokenRequestDTO tokenRequest);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<bool> ValidateRefreshToken(string username, string refreshToken);
        Task SaveRefreshToken(string username, string refreshToken);
        string GenerateRefreshToken();
    }
}

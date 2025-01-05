using Domain.DTOs;
using Domain.Enums;

namespace Application.TokenService
{
    public interface ITokenService
    {
        public string RefreshAccessToken(string accessToken, string refreshToken);
        public bool ValidateToken(string token);
        public Task<TokensGeneratedDTO> GenerateTokens(string email, Role role);

    }
}
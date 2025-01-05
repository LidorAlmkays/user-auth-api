using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTOs;
using Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.TokenService
{
    public class JwtService(IOptions<JWTSettings> options) : ITokenService
    {
        private readonly string SecretKey = options.Value.JWTSecret;
        private readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(options.Value.AccessTokenTime);
        private readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(options.Value.RefreshTokenTime);

        public async Task<TokensGeneratedDTO> GenerateTokens(string email, Role role)
        {
            return new()
            {
                AccessToken =
                GenerateToken(email, role, AccessTokenLifetime),
                RefreshToken = GenerateToken(email, role, RefreshTokenLifetime)
            };
        }

        public string RefreshAccessToken(string accessToken, string refreshToken)
        {
            if (!ValidateToken(refreshToken))
            {
                throw new SecurityTokenException("Invalid refresh token.");
            }

            if (ValidateToken(accessToken))
            {
                throw new InvalidOperationException("Access token is still valid.");
            }
            string email = GetEmailFromToken(refreshToken) ?? throw new Exception("Email not found for the provided token."); ;
            Role role = GetRoleFromToken(refreshToken) ?? throw new Exception("Role not found for the provided token.");
            return GenerateToken(email, role, AccessTokenLifetime);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateToken(string email, Role role, TimeSpan lifetime)
        {
            var credentials = new SigningCredentials(EncodeSecretKey(), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role,RoleExtensions.ToFriendlyString( role))
        };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(lifetime),
                signingCredentials: credentials,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string? GetEmailFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        private Role? GetRoleFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return RoleExtensions.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
        }

        private SymmetricSecurityKey EncodeSecretKey()
        {

            // Convert the secret key to bytes
            byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);

            // Ensure the key is at least 256 bits (32 bytes)
            if (keyBytes.Length < 32)
            {
                int paddingLength = 32 - keyBytes.Length;
                byte[] padding = new byte[paddingLength];

                // Fill padding with a fixed value (e.g., 0)
                for (int i = 0; i < padding.Length; i++)
                {
                    padding[i] = 0; // You can change this value if needed
                }

                // Combine the original key with the padding
                byte[] newKeyBytes = new byte[32];
                Buffer.BlockCopy(keyBytes, 0, newKeyBytes, 0, keyBytes.Length); // Copy original key
                Buffer.BlockCopy(padding, 0, newKeyBytes, keyBytes.Length, padding.Length); // Add padding

                keyBytes = newKeyBytes;
            }

            // If the key is too long, truncate it to 256 bits (32 bytes)
            if (keyBytes.Length > 32)
            {
                byte[] truncatedKey = new byte[32];
                Buffer.BlockCopy(keyBytes, 0, truncatedKey, 0, 32);
                keyBytes = truncatedKey;
            }

            return new SymmetricSecurityKey(keyBytes);
        }

    }

}
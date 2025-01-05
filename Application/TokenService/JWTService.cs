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
                Authorization =
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
                var key = EncodeSecretKey();


                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key, // Use the key that was used for signing
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero, // No clock skew allowed
                    ValidateIssuer = false,    // Skip issuer validation
                    ValidateAudience = false,  // Skip audience validation
                };

                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true; // Token is valid
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false; // Token is invalid
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return false; // General error
            }
        }

        private string GenerateToken(string email, Role role, TimeSpan lifetime)
        {
            var credentials = new SigningCredentials(EncodeSecretKey(), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role,RoleExtensions.ToFriendlyString( role)),
            new Claim("kid","V1")
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

        public Role? GetRoleFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return RoleExtensions.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
        }

        private SymmetricSecurityKey EncodeSecretKey()
        {

            // Convert the secret key to bytes
            byte[] keyBytes = Encoding.UTF8.GetBytes(SecretKey);

            // Ensure the key is 256 bits (32 bytes). If it's shorter, hash it to fit the requirement.
            if (keyBytes.Length < 32)
            {
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    keyBytes = sha256.ComputeHash(keyBytes); // Hash to get exactly 32 bytes
                }
            }
            else if (keyBytes.Length > 32)
            {
                // If the key is too long, use the first 32 bytes.
                keyBytes = keyBytes.Take(32).ToArray();
            }

            return new SymmetricSecurityKey(keyBytes);
        }

    }

}
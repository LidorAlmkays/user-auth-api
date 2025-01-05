using Application.TokenService;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace REST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController(ITokenService tokenService) : ControllerBase
    {
        private readonly ITokenService tokenService = tokenService;

        [HttpGet("refresh-access-token")]
        public ActionResult<string> IsUserTokenValid()
        {
            StringValues refreshToken, accessToken;
            // Extract the refresh-token from the headers
            if (!Request.Headers.TryGetValue("Authorization", out accessToken) || accessToken == StringValues.Empty)
            {
                // Return a bad request if the Authorization token is not found
                return BadRequest("Access token not found in the request headers.");
            }
            if (!Request.Headers.TryGetValue("refresh-token", out refreshToken) || refreshToken == StringValues.Empty)
            {
                // Return a bad request if the refresh-token is not found
                return BadRequest("Refresh token not found in the request headers.");
            }
            try
            {
                string newAccessToken = tokenService.RefreshAccessToken(accessToken, refreshToken);
                return Ok(newAccessToken);
            }
            catch (SecurityTokenException)
            {
                return Conflict("The current access token is still valid. No need to refresh.");
            }
        }


    }
}
using Application.TokenService;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace REST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController(ITokenService tokenService) : ControllerBase
    {
        private readonly ITokenService tokenService = tokenService;

        [HttpPost("validate")]
        public ActionResult IsUserTokenValid(string token)
        {

            if (tokenService.ValidateToken(token))
                return Ok();
            else
                return BadRequest("Missing access or refresh token");
        }
    }
}
using Application.EncryptingService;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace REST.Controllers
{
    [Route("[controller]")]
    public class AuthenticationController(IEncryptingService authService) : Controller
    {
        private readonly IEncryptingService authService = authService ?? throw new ArgumentNullException(nameof(authService));

        [HttpPost]
        public ActionResult<string> AuthenticateUser([FromBody] GenerateTokenInfoDTO generateTokenInfoDTO)
        {

            return Ok(authService.GenerateToken(generateTokenInfoDTO));
        }

    }
}
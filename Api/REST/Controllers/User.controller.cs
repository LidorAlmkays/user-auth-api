using Microsoft.AspNetCore.Mvc;
using Domain.DTOs;
using Application.UserAuthService;
using Domain.Entities;

namespace MyApp.Namespace
{
    [Route("[controller]")]
    [ApiController]
    public class UserController(IUserAuthService userAuthService) : ControllerBase
    {
        private readonly IUserAuthService userAuthService = userAuthService;

        [HttpGet]
        public ActionResult<User> GetUserByEmail([FromQuery] UserIdentifyingInformationDTO userInformation)
        {
            if (string.IsNullOrEmpty(userInformation.Email))
            {
                return new BadRequestObjectResult("Email is required.");
            }

            try
            {
                var a = userAuthService.GetUserByEmail(userInformation.Email);
                return Ok(a);

            }
            catch (ArgumentException ex)
            {
                // Return a 404 Not Found with the exception message
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                // Return a 500 Internal Server Error for unexpected exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }



        [HttpPost]
        public IActionResult RegisterUser([FromBody] UserRegisterDTO userRegisterDTO)
        {

            return Ok();
        }
    }
}

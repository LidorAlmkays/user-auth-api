using Microsoft.AspNetCore.Mvc;
using Domain.DTOs;
using Application.UserService;
using Domain.Entities;

namespace MyApp.Namespace
{
    [Route("[controller]")]
    [ApiController]
    public class UserController(IUserService userAuthService) : ControllerBase
    {
        private readonly IUserService userAuthService = userAuthService;

        [HttpGet]
        public ActionResult<User> GetUserByEmail([FromQuery] UserIdentifyingInformationDTO userInformation)
        {
            if (string.IsNullOrEmpty(userInformation.Email))
            {
                return new BadRequestObjectResult("Email is required.");
            }

            try
            {
                var user = userAuthService.GetUserByEmail(userInformation.Email);
                if (user == null)
                {
                    return NotFound($"User with the email: ${userInformation.Email}, was not found.");
                }
                return Ok(user);

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
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDTO userRegisterDTO)
        {
            try
            {
                await userAuthService.RegisterUser(userRegisterDTO);
                return Ok();

            }
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "A user with the email already exists.");
            }
            catch (Exception err)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while registering the user.\nError: " + err.Message);
            }
        }
    }
}

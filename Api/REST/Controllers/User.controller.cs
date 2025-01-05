using Microsoft.AspNetCore.Mvc;
using Domain.DTOs;
using Application.UserService;
using Domain.Entities;

namespace MyApp.Namespace
{
    [Route("[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;

        //TODO(lidor):Add checks on this route! that a user has an admin role!!!
        [HttpGet]
        public ActionResult<User> GetUserByEmail([FromQuery] UserIdentifyingInformationDTO userInformation)
        {
            if (string.IsNullOrEmpty(userInformation.Email))
            {
                return new BadRequestObjectResult("Email is required.");
            }

            try
            {
                var user = userService.GetUserByEmail(userInformation.Email);
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

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDTO userRegisterDTO)
        {
            try
            {
                await userService.RegisterUser(userRegisterDTO);
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

        [HttpPost("login")]
        public async Task<ActionResult<TokensGeneratedDTO>> Login(UserLoginDTO userInformation)
        {

            try
            {
                TokensGeneratedDTO tokens = await userService.Login(userInformation);
                if (tokens != null)
                    return Ok(tokens);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user tokens.");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
        }
    }
}

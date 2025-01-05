using Application.TokenService;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace REST.Guards
{
    public class TokenValidationGuard : Attribute, IAsyncActionFilter
    {
        private readonly Role _requiredRole;
        private readonly ITokenService _tokenValidationService;

        public TokenValidationGuard(Role requiredRole, ITokenService tokenValidationService)
        {
            _requiredRole = requiredRole;
            _tokenValidationService = tokenValidationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            // Extract tokens from headers
            var token = request.Headers["Authorization"].ToString();
            var refreshToken = request.Headers["Refresh-Token"].ToString();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Missing tokens in headers." });
                return;
            }

            // Validate token
            var isValid = _tokenValidationService.ValidateToken(token);
            if (!isValid)
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Invalid or expired access token." });
                return;
            }

            // Check role
            var role = _tokenValidationService.GetRoleFromToken(token);
            if (role == null || role != _requiredRole)
            {
                context.Result = new JsonResult(new { Message = $"Insufficient permissions. Required role: {_requiredRole}." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            // Proceed to the next middleware/action
            await next();
        }
    }
}
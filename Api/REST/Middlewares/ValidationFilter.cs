using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace REST.Middlewares
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid:");
                foreach (var key in context.ModelState.Keys)
                {
                    Console.WriteLine($"{key}: {context.ModelState[key].Errors.Count} errors");
                }
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }

}
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Middleware
{
    public class Roles
    {
        private readonly RequestDelegate _next;

        public Roles(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the user is authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // You can log the role here for debugging purposes if you like
                var userRole = context.User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            }

            // Continue the request pipeline (don't block anything)
            await _next(context);
        }
    }
}
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
                // Get the user's role from claims
                var userRole = context.User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                // Determine if the user is trying to access an Admin-only area
                var isAdminRoute = context.Request.Path.StartsWithSegments("/Admin");

                // Block access if a non-admin user tries to access an admin-only page
                if (isAdminRoute && userRole != "Admin")
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Access Denied: Admins only.");
                    return;
                }
            }

            // Continue the request pipeline
            await _next(context);
        }
    }
}

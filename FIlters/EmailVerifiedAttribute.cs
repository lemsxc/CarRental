using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using CarRental.Services;
using System.Threading.Tasks;

namespace CarRental.Filters
{
    public class EmailVerifiedOnlyAttribute : TypeFilterAttribute
    {
        public EmailVerifiedOnlyAttribute() : base(typeof(EmailVerifiedOnlyFilter))
        {
        }

        private class EmailVerifiedOnlyFilter : IAsyncActionFilter
        {
            private readonly ApplicationDbContext _context;

            public EmailVerifiedOnlyFilter(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var session = context.HttpContext.Session;
                var email = session.GetString("Email");

                if (!string.IsNullOrEmpty(email))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user != null && !user.IsVerified)
                    {
                        context.Result = new RedirectToActionResult("Verify", "Auth", null);
                        return;
                    }
                }

                await next();
            }
        }
    }
}

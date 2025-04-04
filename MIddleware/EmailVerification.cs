using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CarRental.Models;
using CarRental.Services;

namespace CarRental.Middleware
{

    public class EmailVerification
    {
        private readonly RequestDelegate _next;
        private readonly ApplicationDbContext _context;

        public EmailVerification(RequestDelegate next, ApplicationDbContext context)
        {
            _next = next;
            _context = context;
        }

        public async Task Invoke(HttpContext context)
        {
            var userEmail = context.Session.GetString("Email"); // Assuming session stores user email

            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user != null && !user.IsVerified)
                {
                    context.Response.Redirect("/Auth/Verify");
                    return;
                }
            }

            await _next(context);
        }
    }
}

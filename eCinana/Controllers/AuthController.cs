using eCinana.Models;
using eCinana.Models.DbModels;
using eCinana.Models.FormModels;
using eCinana.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eCinana.Controllers
{
    public class AuthController : BaseController
    {
        private readonly MainDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IClaimsHelper _claimsHelper;

        public AuthController(
            MainDbContext context,
            IConfiguration configuration,
            IJwtTokenService jwtTokenService,
            IClaimsHelper claimsHelper)
        {
            _context = context;
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
            _claimsHelper = claimsHelper;
        }

        #region Login
        public async Task<IActionResult> Login([FromBody] LoginFM form)
        {
            try
            {
                // Hash the incoming password for comparison
                string hashedPassword = HashPassword(form.txt_Password);

                // Retrieve the user with the matching username and password hash
                var dbuser = await _context.Users
                                .Where(e => e.username == form.txt_Username && e.password_hash == hashedPassword)
                                .FirstOrDefaultAsync();

                // Check if user exists
                if (dbuser == null)
                {
                    ModelState.AddModelError("txt_Username", "Invalid username or password.");
                    return Unauthorized(GenerateErrorResponse());
                }

                // Get the user's role
                string role = dbuser.role;

                // Generate JWT token using user data and role
                var token = _jwtTokenService.GenerateToken(dbuser.username, role);

                // Set the token as a secure cookie
                Response.Cookies.Append("eCinema_jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                return Json(new
                {
                    success = true,
                    text = "Login successful.",
                    user = new
                    {
                        dbuser.username,
                        dbuser.role,
                        dbuser.full_name,
                        dbuser.email,
                        jwt = token
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    text = $"An unexpected error occurred: {ex.Message} {ex.InnerException?.Message ?? ""}"
                });
            }
        }
        #endregion

        #region Log out
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Response.Cookies.Append("eCinema_jwt", string.Empty, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(-1)
                });

                return Json(new
                {
                    success = true,
                    text = "You have been successfully logged out."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    text = $"An unexpected error occurred: {ex.Message} {ex.InnerException?.Message ?? ""}"
                });
            }
        }
        #endregion
    }
}

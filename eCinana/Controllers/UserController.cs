using eCinana.Models;
using eCinana.Models.DbModels;
using eCinana.Models.FormModels;
using eCinana.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace eCinana.Controllers
{
    public class UserController : BaseController
    {
        private readonly MainDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IClaimsHelper _claimsHelper;
        private readonly EmailService _email;
        private readonly JwtSettings _jwtSettings;

        public UserController(MainDbContext context,
            IConfiguration configuration,
            IJwtTokenService jwtTokenService,
            IClaimsHelper claimsHelper,
            EmailService email,
            IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
            _claimsHelper = claimsHelper;
            _email = email;
            _jwtSettings = jwtSettings.Value;
        }
        #region Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserFM form)
        {
			try
			{
                // Validate form input and check for errors
                if (!ModelState.IsValid || await ValCreate(form))
                {
                    return BadRequest(GenerateErrorResponse());
                }

                // Create new User object and set properties
                var user = new User
                {
                    username = form.txt_Username,
                    password_hash = HashPassword(form.txt_Password),
                    role = form.txt_Role,
                    full_name = form.txt_FullName,
                    email = form.txt_Email,
                    phone_number = form.txt_PhoneNumber,
                    status = form.txt_Status ? "Active" : "Inactive"
                };

                // Save user to the database asynchronously
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, text = "User created successfully.", user });
            }
            catch (Exception ex)
			{
                return StatusCode(500, new { success = false,
                    text = $"An unexpected error occurred: {ex.Message} {ex.InnerException?.Message ?? ""}"
                });
            }
        }

        private async Task<bool> ValCreate(UserFM form)
        {
            bool result = false;
            if (await ValRole(form.txt_Role))
            {
                return true;
            }
            else
            {
                if (await ValUsernameUnique(form.txt_Username, form.txt_UserId))
                {
                    ModelState.AddModelError("txt_Username", "The username is already in use. Please choose a different username.");
                    result = true;
                }
                if (await ValEmailUnique(form.txt_Email, form.txt_UserId))
                {
                    ModelState.AddModelError("txt_Username", "The email is already in use. Please choose a different email.");
                    result = true;
                }
                if (await ValPassword(form.txt_Password, form.txt_Username))
                {
                    result = true;
                }
            }
            return result;
        }

        private async Task<bool> ValUsernameUnique(string txt_Username, int txt_UserId)
        {
            if (txt_UserId == 0)
            {
                return await _context.Users
                    .AnyAsync(u => u.username == txt_Username);
            }
            else
            {
                return await _context.Users
                    .AnyAsync(u => u.username == txt_Username && u.user_id != txt_UserId);
            }
        }
        private async Task<bool> ValEmailUnique(string txt_Email, int txt_UserId)
        {
            if (txt_UserId == 0)
            {
                return await _context.Users
                    .AnyAsync(u => u.email == txt_Email);
            }
            else
            {
                return await _context.Users
                    .AnyAsync(u => u.email == txt_Email && u.user_id != txt_UserId);
            }
        }

        private async Task<bool> ValPassword(string txt_Password, string txt_Username)
        {
            bool result = false;

            // Check for at least one number
            if (!txt_Password.Any(char.IsDigit))
            {
                ModelState.AddModelError("txt_Password", "Password must contain at least one number.");
                result = true;
            }
            // Check for at least one lowercase letter
            if (!txt_Password.Any(char.IsLower))
            {
                ModelState.AddModelError("txt_Password", "Password must contain at least one lowercase letter.");
                result = true;
            }
            // Check for at least one uppercase letter
            if (!txt_Password.Any(char.IsUpper))
            {
                ModelState.AddModelError("txt_Password", "Password must contain at least one uppercase letter.");
                result = true;
            }
            // Check for at least one special character
            if (!txt_Password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                ModelState.AddModelError("txt_Password", "Password must contain at least one special symbol.");
                result = true;
            }
            // Check that password doesn't contain non-English characters (e.g., Thai, Russian, etc.)
            if (txt_Password.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch) && !IsEnglishCharacter(ch)))
            {
                ModelState.AddModelError("txt_Password", "Password must only contain English letters and special symbols.");
                result = true;
            }
            // Check that password doesn't contain the username
            if (txt_Password.Contains(txt_Username, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("txt_Password", "Password cannot contain the username.");
                result = true;
            }
            // Check that password does not match date patterns like 'yyyyMMdd' or 'ddMMyyyy'
            if (IsDatePattern(txt_Password))
            {
                ModelState.AddModelError("txt_Password", "Password cannot match common date patterns (e.g., 'yyyyMMdd', 'ddMMyyyy').");
                result = true;
            }
            return result;
        }

        private async Task<bool> ValRole(string txt_Role)
        {
            bool result = false;
            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(currentUserRole) || currentUserRole == "Customer")
            {
                if (txt_Role != "Customer")
                {
                    ModelState.AddModelError("txt_Role", "You are only allowed to create users with the 'Customer' role.");
                    result = true;
                }
            }
            return result;
        }

        private bool IsEnglishCharacter(char c)
        {
            return (char.IsLetter(c) && (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')) ||
          char.IsDigit(c) ||
          "!@#$%^&*()_+=-[]{}|;:'\",.<>?/".Contains(c);
        }
        // Helper method to check if password matches common date patterns like 'yyyyMMdd' or 'ddMMyyyy'
        private bool IsDatePattern(string password)
        {
            string[] datePatterns = new[]
                {
                    @"\d{4}\d{2}\d{2}", // Matches yyyyMMdd (e.g., 20231123)
                    @"\d{2}\d{2}\d{4}", // Matches ddMMyyyy (e.g., 23112024)
                    @"\d{2}\d{2}\d{2}", // Matches ddMMyy (e.g., 231124)
                    @"\d{4}-\d{2}-\d{2}", // Matches yyyy-MM-dd (e.g., 2023-11-23)
                };

            return datePatterns.Any(pattern => Regex.IsMatch(password, pattern));
        }
        #endregion

        #region vUpdate
        [HttpGet]
        public async Task<IActionResult> vUpdate([FromBody] UserFM form)
        {
            try
            {
                var user = await ValUserExist(form.txt_UserId, form.txt_Username);
                if (user == null)
                {
                    throw new Exception($"User with ID {form.txt_UserId} and username '{form.txt_Username}' does not exist.");
                }

                form.txt_Role = user.role;
                form.txt_FullName = user.full_name;
                form.txt_Email = user.email;
                form.txt_PhoneNumber = user.phone_number;
                form.txt_Status = (user.status == "Active");

                return Json(new { success = true, form });
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
        public async Task<User?> ValUserExist(int userId, string username)
        {
            return await _context.Users
                                 .Where(e => e.user_id == userId && e.username == username)
                                 .FirstOrDefaultAsync();
        }

        #endregion

        #region Update
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserFM form)
        {
            try
            {
                if (!ModelState.IsValid || await ValUpdate(form))
                {
                    return BadRequest(GenerateErrorResponse());
                }

                var dbuser = await _context.Users.FindAsync(form.txt_UserId);
                var user = new User
                {
                    user_id = form.txt_UserId,
                    username = form.txt_Username,
                    password_hash = HashPassword(form.txt_Password),
                    role = form.txt_Role,
                    full_name = form.txt_FullName,
                    email = form.txt_Email,
                    phone_number = form.txt_PhoneNumber,
                    status = form.txt_Status ? "Active" : "Inactive",
                    registration_date = dbuser.registration_date
                };

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, text = "User update successfully.", user });
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
        private async Task<bool> ValUpdate(UserFM form)
        {
            bool result = false;
            if (await ValRole(form.txt_Role))
            {
                return true;
            }
            else
            {
                var userExists = await ValUserExist(form.txt_UserId, form.txt_Username);
                if (userExists == null)
                {
                    ModelState.AddModelError("txt_UserId", "The user with the provided ID does not exist.");
                    result = true;
                }

                if (await ValUsernameUnique(form.txt_Username, form.txt_UserId))
                {
                    ModelState.AddModelError("txt_Username", "The username is already in use. Please choose a different username.");
                    result = true;
                }
                if (await ValEmailUnique(form.txt_Email, form.txt_UserId))
                {
                    ModelState.AddModelError("txt_Username", "The email is already in use. Please choose a different email.");
                    result = true;
                }
                if (await ValPassword(form.txt_Password, form.txt_Username))
                {
                    result = true;
                }
                if (await ValSelfUsername(form.txt_UserId))
                {
                    result = true;
                }
            }
            return result;
        }
        private async Task<bool> ValSelfUsername(int txt_UserId)
        {
            bool result = false;
            string usernameClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usernameClaim))
            {
                throw new Exception("User not authenticated.");
            }
            var dbuser = await _context.Users.FirstOrDefaultAsync(u => u.username == usernameClaim);
            if (dbuser == null)
            {
                throw new Exception("User not found.");
            }

            if (txt_UserId != dbuser.user_id)
            {
                throw new Exception("User ID does not match the authenticated user.");
            }
            return result;
        }
        #endregion

        #region Inactive
        [HttpDelete]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Inactive([FromBody] UserFM form)
        {
            try
            {
                var user = await ValUserExist(form.txt_UserId, form.txt_Username);
                if (user == null)
                {
                    throw new Exception($"User with ID {form.txt_UserId} and username '{form.txt_Username}' does not exist.");
                }

                user.status = "Inactive";
                await _context.SaveChangesAsync();
                user.password_hash = null;
                return Json(new { success = true, text = "User disabled successfully.", user });
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

        #region Send reset password via email
        [HttpPost]
        public async Task<IActionResult> SetPasswordEmail([FromBody] SetPasswordEmailFM form)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.username == form.txt_Username && u.email == form.txt_Email && u.status == "Active");

                if (user == null)
                {
                    ModelState.AddModelError("txt_Email", "User with provided username and email not found or disabled user.");
                    return BadRequest(GenerateErrorResponse());
                }

                string token = GeneratePasswordResetToken(form.txt_Username);
                string resetLink = $"https://localhost:44343/User/vResetPassword?token={token}";

                bool emailSent = await _email.SendPasswordResetEmail(user.email, resetLink);
                if (!emailSent)
                {
                    ModelState.AddModelError("txt_Email", "Failed to send password reset email.");
                    return BadRequest(GenerateErrorResponse());
                }

                return Json(new { success = true, text = "Password reset email has been sent successfully." });


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

        private string GeneratePasswordResetToken(string txt_Username)
        {
            var token = _jwtTokenService.GenerateResetPasswordToken(txt_Username);
            return token;
        }
        #endregion

        #region vResetPassword
        [HttpGet]
        public async Task<IActionResult> vResetPassword(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { success = false, text = "Token is required." });
                }

                var txt_Username = await ValToken(token);
                if (string.IsNullOrEmpty(txt_Username))
                {
                    return BadRequest(new { success = false, text = "Token is invalid or does not contain a valid username." });
                }

                return Json(new { success = true, form = new { txt_Username, txt_Token = token } });
            }
            catch (SecurityTokenException ex)
            {
                // Invalid token or expired
                return BadRequest(new { success = false, text = "The token has expired." });
            }
            catch (Exception ex)
            {
                // Catch unexpected errors
                return StatusCode(500, new
                {
                    success = false,
                    text = $"An unexpected error occurred: {ex.Message} {ex.InnerException?.Message ?? ""}"
                });
            }
        }

        #endregion

        #region ResetPassword
        [HttpPut]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordFM form)
        {
            try
            {
                if (!ModelState.IsValid || await ValResetPassword(form))
                {
                    return BadRequest(GenerateErrorResponse());
                }

                var txt_Username = await ValToken(form.txt_Token);
                if (string.IsNullOrEmpty(txt_Username))
                {
                    return BadRequest(new { success = false, text = "Token is invalid or does not contain a valid username." });
                }

                var user = await _context.Users.Where(e => e.username == form.txt_Username).FirstOrDefaultAsync();
                if (user == null)
                {
                    return BadRequest(new { success = false, text = "User not found." });
                }

                user.password_hash = HashPassword(form.txt_Password);
                await _context.SaveChangesAsync();

                return Json(new { success = true, text = "Password reset successfully." });
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(new { success = false, text = "The token has expired." });
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

        private async Task<bool> ValResetPassword(ResetPasswordFM form)
        {
            bool result = false;
            if (await ValPassword(form.txt_Password, form.txt_Username))
            {
                result = true;
            }
            return result;
        }

        private async Task<string> ValToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // Optional: remove default clock skew tolerance (usually 5 minutes)
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    return jwtToken?.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                }

                return null;
            }
            catch (SecurityTokenException)
            {
                return null; // Invalid token
            }
        }
        #endregion
    }
}

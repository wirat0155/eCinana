using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace eCinana.Controllers
{
    public class BaseController : Controller
    {
        protected JsonResult GenerateErrorResponse()
        {
            var errorMessages = ModelState.Keys
                .Where(key => ModelState[key].Errors.Any())
                .SelectMany(key => ModelState[key].Errors.Select((error, index) =>
                {
                    var errorMessage = error.ErrorMessage;

                    if (key.Contains("["))
                    {
                        var prefix = key.Substring(0, key.IndexOf("["));
                        var formattedKey = prefix + key.Substring(key.IndexOf("["))
                                                       .Replace("[", "_")
                                                       .Replace("]", "");
                        return new { property = formattedKey, errorMessage };
                    }
                    else
                    {
                        return new { property = key, errorMessage };
                    }
                }))
                .ToList();

            if (errorMessages.Count == 0)
            {
                errorMessages.Add(new { property = "txt_form", errorMessage = "Invalid data." });
            }

            return Json(new { success = false, errors = errorMessages });
        }

        protected string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                foreach (byte byteValue in bytes)
                {
                    builder.Append(byteValue.ToString("x2")); // convert byte to hexadecimal string
                }

                return builder.ToString(); // The hashed password
            }
        }
    }
}

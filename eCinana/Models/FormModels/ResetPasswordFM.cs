using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.FormModels
{
    public class ResetPasswordFM
    {
        [Required]
        public string txt_Token { get; set; }
        [Required]
        public string txt_Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string txt_Password { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [Compare("txt_Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string txt_RePassword { get; set; }
    }

}

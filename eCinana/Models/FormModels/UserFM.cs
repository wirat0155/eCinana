using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.FormModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserFM
    {
        [Required]
        public int txt_UserId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Username must be between 8 and 100 characters.")]
        public string txt_Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string txt_Password { get; set; }

        [Required]
        [RegularExpression(@"^(Customer|Manager|Cashier|Admin)$", ErrorMessage = "Role must be one of the following: Customer, Manager, Cashier, Admin.")]
        public string txt_Role { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Full Name must be mot more than 100 characters long.")]
        public string txt_FullName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string txt_Email { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "Phone number must be between 9 and 20 characters.")]
        public string txt_PhoneNumber { get; set; }

        [Required]
        public bool txt_Status { get; set; } = true;
    }

}

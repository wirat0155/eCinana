using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.FormModels
{
    public class LoginFM
    {
        [Required]
        public string txt_Username { get; set; }
        [Required]
        public string txt_Password { get; set; }
    }
}

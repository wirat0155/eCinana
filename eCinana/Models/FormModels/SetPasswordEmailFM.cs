using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.FormModels
{
    public class SetPasswordEmailFM
    {
        [Required]
        public string txt_Username { get; set; }
        [Required]
        public string txt_Email {  get; set; }
    }
}

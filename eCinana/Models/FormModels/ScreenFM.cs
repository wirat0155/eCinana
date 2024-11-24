using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.FormModels
{
    public class ScreenFM
    {
        [Key]
        public int txt_ScreenId { get; set; }

        [Required(ErrorMessage = "Screen number is required.")]
        public int txt_ScreenNumber { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(6, 3000, ErrorMessage = "Capacity must be between 6 and 3000.")]
        public int txt_Capacity { get; set; }

        [MaxLength(50, ErrorMessage = "Sound system cannot exceed 50 characters.")]
        public string txt_SoundSystem { get; set; }

        [MaxLength(50, ErrorMessage = "Format cannot exceed 50 characters.")]
        [RegularExpression(@"^(IMAX|3D|2D)$", ErrorMessage = "Format must be either 'IMAX', '3D', or '2D'.")]
        public string txt_Format { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        [RegularExpression(@"^(Maintenance|Available)$", ErrorMessage = "Status must be 'Maintenance' or 'Available'.")]
        public string txt_Status { get; set; }
    }
}

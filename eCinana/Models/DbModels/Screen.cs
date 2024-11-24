using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.DbModels
{
    public class Screen
    {
        [Key]
        public int screen_id { get; set; }

        [Required]
        public int screen_number { get; set; }

        [Required]
        public int capacity { get; set; }

        [MaxLength(50)]
        public string sound_system { get; set; }

        [MaxLength(50)]
        public string format { get; set; }

        [MaxLength(50)]
        public string status { get; set; } = "Available";
    }

}

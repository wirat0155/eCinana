using System.ComponentModel.DataAnnotations;
using System;

namespace eCinana.Models.DbModels
{
    public class User
    {
        [Key]
        public int user_id { get; set; }

        [Required]
        [MaxLength(100)]
        public string username { get; set; }

        [Required]
        [MaxLength(255)]
        public string password_hash { get; set; }

        [Required]
        [MaxLength(50)]
        public string role { get; set; }

        [MaxLength(150)]
        public string full_name { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string email { get; set; }

        [MaxLength(20)]
        public string phone_number { get; set; }

        [Required]
        public DateTime registration_date { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string status { get; set; } = "Active";
    }


}

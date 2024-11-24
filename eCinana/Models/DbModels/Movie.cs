using System.ComponentModel.DataAnnotations;
using System;

namespace eCinana.Models.DbModels
{
    public class Movie
    {
        [Key]
        public int movie_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string title { get; set; }

        [MaxLength(100)]
        public string genre { get; set; }

        public string description { get; set; }

        public decimal rating { get; set; }

        public int duration_minutes { get; set; }

        public DateTime release_date { get; set; }

        [MaxLength(255)]
        public string poster_url { get; set; }

        [MaxLength(255)]
        public string trailer_url { get; set; }

        [Required]
        [MaxLength(50)]
        public string status { get; set; }
    }

}

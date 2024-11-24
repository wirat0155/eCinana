using System.ComponentModel.DataAnnotations;
using System;

namespace eCinana.Models.DbModels
{
    public class Showtime
    {
        public int ShowtimeId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public int ScreenId { get; set; }

        [Required]
        public DateTime ShowDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        // Navigation properties
        public Movie Movie { get; set; }
        public Screen Screen { get; set; }
    }

}

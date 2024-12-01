using System.ComponentModel.DataAnnotations;
using System;

namespace eCinana.Models.DbModels
{
    public class Showtime
    {
        [Key]
        public int showtime_id { get; set; }

        [Required]
        public int movie_id { get; set; }

        [Required]
        public int screen_id { get; set; }

        [Required]
        public DateTime show_date { get; set; }

        [Required]
        public DateTime start_time { get; set; }

        [Required]
        public DateTime end_time { get; set; }

        //// Navigation properties
        //public Movie Movie { get; set; }
        //public Screen Screen { get; set; }
    }

}

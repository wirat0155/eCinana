using System.ComponentModel.DataAnnotations;
using System;

namespace eCinana.Models.DbModels
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        [Required]
        public DateTime FeedbackDate { get; set; } = DateTime.Now;

        // Navigation properties
        public Movie Movie { get; set; }
        public User User { get; set; }
    }

}

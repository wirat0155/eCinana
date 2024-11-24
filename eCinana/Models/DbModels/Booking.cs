using System.ComponentModel.DataAnnotations;
using System;

namespace eCinana.Models.DbModels
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ShowtimeId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentStatus { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Showtime Showtime { get; set; }
    }

}

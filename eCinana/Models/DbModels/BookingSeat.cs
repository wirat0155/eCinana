using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.DbModels
{
    public class BookingSeat
    {
        public int BookingSeatId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int SeatId { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
        public Seat Seat { get; set; }
    }

}

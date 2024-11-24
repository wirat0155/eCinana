using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.DbModels
{
    public class Seat
    {
        public int SeatId { get; set; }

        [Required]
        public int ScreenId { get; set; }

        [Required]
        [MaxLength(10)]
        public string SeatNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string SeatType { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Navigation property
        public Screen Screen { get; set; }
    }

}

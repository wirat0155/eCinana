using System.ComponentModel.DataAnnotations;
using System;

namespace eCinana.Models.DbModels
{
    public class Order
    {
        public int OrderId { get; set; }

        public int? BookingId { get; set; } // Nullable if the order is not tied to a booking

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalPrice { get; set; }

        // Navigation property
        public Booking Booking { get; set; }
    }

}

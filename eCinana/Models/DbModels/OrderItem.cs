using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.DbModels
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation properties
        public Order Order { get; set; }
        public ConcessionItem ConcessionItem { get; set; }
    }

}

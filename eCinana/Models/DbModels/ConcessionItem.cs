using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.DbModels
{
    public class ConcessionItem
    {
        public int ItemId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ItemName { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }
    }

}

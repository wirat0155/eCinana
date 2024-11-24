using System.ComponentModel.DataAnnotations;
using System;

namespace eCinana.Models.FormModels
{
    public class MovieFM
    {
        [Key]
        public int txt_MovieId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 255 characters.")]
        public string txt_Title { get; set; }

        [MaxLength(100, ErrorMessage = "Genre cannot exceed 100 characters.")]
        public string txt_Genre { get; set; }

        public string txt_Description { get; set; }

        [Range(0.1, 10.0, ErrorMessage = "Rating must be between 0.1 and 10.0.")]
        public decimal txt_Rating { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int txt_DurationMinutes { get; set; }

        public DateTime txt_ReleaseDate { get; set; }

        [MaxLength(255, ErrorMessage = "Poster URL cannot exceed 255 characters.")]
        public string txt_PosterUrl { get; set; }

        [MaxLength(255, ErrorMessage = "Trailer URL cannot exceed 255 characters.")]
        public string txt_TrailerUrl { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        [RegularExpression("Archived|Upcoming|Now Showing", ErrorMessage = "Status must be 'Archived', 'Upcoming', or 'Now Showing'.")]
        public string txt_Status { get; set; }
    }
}

using eCinana.Models.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.ViewModels
{
    public class FilterMovieVM : IValidatableObject
    {
        public string txt_Genre { get; set; }
        [Range(0.1, 10.0, ErrorMessage = "Rating From must be between 0.1 and 10.0.")]

        public decimal? txt_RatingFrom { get; set; }
        [Range(0.1, 10.0, ErrorMessage = "Rating To must be between 0.1 and 10.0.")]

        public decimal? txt_RatingTo { get; set; }
        public DateTime? txt_ReleaseDateFrom {  get; set; }
        public DateTime? txt_ReleaseDateTo { get; set; }
        public List<Movie> dt_MovieList { get; set; } = new List<Movie>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            // Validate rating range
            if (txt_RatingFrom.HasValue && txt_RatingTo.HasValue)
            {
                if (txt_RatingFrom > txt_RatingTo)
                {
                    validationResults.Add(new ValidationResult(
                        "Rating From must be less than or equal to Rating To.",
                        new[] { nameof(txt_RatingFrom), nameof(txt_RatingTo) }
                    ));
                }
            }

            // Validate release date range
            if (txt_ReleaseDateFrom.HasValue && txt_ReleaseDateTo.HasValue)
            {
                if (txt_ReleaseDateFrom > txt_ReleaseDateTo)
                {
                    validationResults.Add(new ValidationResult(
                        "Release Date From must be less than or equal to Release Date To.",
                        new[] { nameof(txt_ReleaseDateFrom), nameof(txt_ReleaseDateTo) }
                    ));
                }
            }

            return validationResults;
        }
    }

}

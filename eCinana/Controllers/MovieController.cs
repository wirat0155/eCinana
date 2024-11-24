using eCinana.Models;
using eCinana.Models.DbModels;
using eCinana.Models.FormModels;
using eCinana.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eCinana.Controllers
{
    [Authorize(Roles = "Manager, Admin")]
    public class MovieController : BaseController
    {
        private readonly MainDbContext _context;

        public MovieController(MainDbContext context)
        {
            _context = context;
        }
        #region Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovieFM form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(GenerateErrorResponse());
                }

                Movie movie = new Movie()
                {
                    title = form.txt_Title,
                    genre = form.txt_Genre,
                    description = form.txt_Description,
                    rating = form.txt_Rating,
                    duration_minutes = form.txt_DurationMinutes,
                    release_date = form.txt_ReleaseDate,
                    poster_url = form.txt_PosterUrl,
                    trailer_url = form.txt_TrailerUrl,
                    status = form.txt_Status
                };

                await _context.Movies.AddAsync(movie);
                await _context.SaveChangesAsync();

                return Json(new { success = true, text = "Movie created successfully.", movie });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    text = $"An unexpected error occurred: {ex.Message} {ex.InnerException?.Message ?? ""}"
                });
            }
        }
        #endregion

        #region Update
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MovieFM form)
        {
            try
            {
                if (!ModelState.IsValid || await ValUpdate(form))
                {
                    return BadRequest(GenerateErrorResponse());
                }

                Movie movie = new Movie()
                {
                    movie_id = form.txt_MovieId,
                    title = form.txt_Title,
                    genre = form.txt_Genre,
                    description = form.txt_Description,
                    rating = form.txt_Rating,
                    duration_minutes = form.txt_DurationMinutes,
                    release_date = form.txt_ReleaseDate,
                    poster_url = form.txt_PosterUrl,
                    trailer_url = form.txt_TrailerUrl,
                    status = form.txt_Status
                };

                _context.Movies.Update(movie);
                await _context.SaveChangesAsync();

                return Json(new { success = true, text = "Movie created successfully.", movie });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    text = $"An unexpected error occurred: {ex.Message} {ex.InnerException?.Message ?? ""}"
                });
            }
        }

        private async Task<bool> ValUpdate(MovieFM form)
        {
            bool result = false;
            var userExists = await ValMovieExist(form.txt_MovieId);
            if (userExists == null)
            {
                throw new Exception("Movie not found.");
            }
            return result;
        }


        public async Task<Movie?> ValMovieExist(int txt_MovieId)
        {
            return await _context.Movies
                                 .Where(e => e.movie_id == txt_MovieId)
                                 .FirstOrDefaultAsync();
        }
        #endregion

        #region Delete
        [HttpDelete]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Delete([FromBody] MovieFM form)
        {
            try
            {
                var movie = await ValMovieExist(form.txt_MovieId);
                if (movie == null)
                {
                    throw new Exception($"Movie with ID {form.txt_MovieId} does not exist.");
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                return Json(new { success = true, text = "Movie delete successfully.", movie });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    text = $"An unexpected error occurred: {ex.Message} {ex.InnerException?.Message ?? ""}"
                });
            }
        }
        #endregion

        #region filter
        [HttpGet]
        public async Task<IActionResult> Filter([FromBody] FilterMovieVM form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(GenerateErrorResponse());
                }

                // seperate to vFilter
                form.dt_MovieList = await _context.Movies
                    .Where(e => e.genre.Contains(form.txt_Genre ?? ""))
                    .Where(e => !form.txt_RatingFrom.HasValue || e.rating >= form.txt_RatingFrom)
                    .Where(e => !form.txt_RatingTo.HasValue || e.rating <= form.txt_RatingTo)
                    .Where(e => !form.txt_ReleaseDateFrom.HasValue || e.release_date >= form.txt_ReleaseDateFrom)
                    .Where(e => !form.txt_ReleaseDateTo.HasValue || e.release_date <= form.txt_ReleaseDateTo)
                    .ToListAsync();

                return Json(new { success = true, form });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    text = $"An unexpected error occurred: {ex.Message} {ex.InnerException?.Message ?? ""}"
                });
            }
        }

        #endregion
    }
}

using eCinana.Models;
using eCinana.Models.DbModels;
using eCinana.Models.FormModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eCinana.Controllers
{
    public class ScreenController : BaseController
    {
        private readonly MainDbContext _context;

        public ScreenController(MainDbContext context)
        {
            _context = context;
        }

        #region Create
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Create([FromBody] ScreenFM form)
        {
            try
            {
                if (!ModelState.IsValid || await ValCreate(form))
                {
                    return BadRequest(GenerateErrorResponse());
                }

                Screen screen = new Screen() { 
                    screen_number = form.txt_ScreenNumber,
                    capacity = form.txt_Capacity,
                    sound_system = form.txt_SoundSystem,
                    format = form.txt_Format,
                    status = form.txt_Status
                };

                await _context.Screens.AddAsync(screen);
                await _context.SaveChangesAsync();

                return Json(new { success = true, text = "Screen created successfully.", screen });
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

        private async Task<bool> ValCreate(ScreenFM form)
        {
            bool result = false;
            if (await ValScreenNumberUnique(form.txt_ScreenNumber, form.txt_ScreenId))
            {
                ModelState.AddModelError("txt_ScreenName", "The screen name is already in use. Please choose a different name.");
                result = true;
            }
            return result;
        }
        private async Task<bool> ValScreenNumberUnique(int txt_ScreenNumber, int txt_ScreenId)
        {
            if (txt_ScreenId == 0)
            {
                return await _context.Screens
                    .AnyAsync(u => u.screen_number == txt_ScreenNumber);
            }
            else
            {
                return await _context.Screens
                    .AnyAsync(u => u.screen_number == txt_ScreenNumber && u.screen_id != txt_ScreenId);
            }
        }
        #endregion

        #region Update
        [HttpPut]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Update([FromBody] ScreenFM form)
        {
            try
            {
                if (!ModelState.IsValid || await ValUpdate(form))
                {
                    return BadRequest(GenerateErrorResponse());
                }

                Screen screen = new Screen()
                {
                    screen_id = form.txt_ScreenId,
                    screen_number = form.txt_ScreenNumber,
                    capacity = form.txt_Capacity,
                    sound_system = form.txt_SoundSystem,
                    format = form.txt_Format,
                    status = form.txt_Status
                };

                _context.Screens.Update(screen);
                await _context.SaveChangesAsync();

                return Json(new { success = true, text = "Screen updated successfully.", screen });
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
        private async Task<bool> ValUpdate(ScreenFM form)
        {
            bool result = false;
            var screenExist = await ValScreenExist(form.txt_ScreenId);
            if (screenExist == null)
            {
                throw new Exception("Screen not found.");
            }
            else
            {
                if (await ValScreenNumberUnique(form.txt_ScreenNumber, form.txt_ScreenId))
                {
                    ModelState.AddModelError("txt_ScreenName", "The screen name is already in use. Please choose a different name.");
                    result = true;
                }
            }

            return result;
        }
        public async Task<Screen?> ValScreenExist(int txt_ScreenId)
        {
            return await _context.Screens
                                 .Where(e => e.screen_id == txt_ScreenId)
                                 .FirstOrDefaultAsync();
        }
        #endregion

        #region Delete
        [HttpDelete]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Delete([FromBody] ScreenFM form)
        {
            try
            {
                var screen = await ValScreenExist(form.txt_ScreenId);
                if (screen == null)
                {
                    throw new Exception("Screen not found.");
                }

                _context.Screens.Remove(screen);
                await _context.SaveChangesAsync();

                return Json(new { success = true, text = "Screen deleted successfully.", screen });
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

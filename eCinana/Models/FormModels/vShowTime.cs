using System;
using System.ComponentModel.DataAnnotations;

namespace eCinana.Models.FormModels
{
    public class vShowTime
    {
        [Required(ErrorMessage = "Start Time is required.")]
        public DateTime start_time { get; set; }
        [Required(ErrorMessage = "End Time is required.")]
        public DateTime end_time { get; set; }
        public int screen_id { get; set; }


        public vShowTime()
        {
            var now = DateTime.Now; 
            start_time = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0); 
            end_time = start_time.AddDays(1);
        }
    }
}

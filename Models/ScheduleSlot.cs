using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace TurboReserve.Models
{
    public class ScheduleSlot
    {
        public int Id { get; set; }
        public int ServiceProviderId { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        [EndTimeAfterStartTime(nameof(StartTime), ErrorMessage = "EndTime must be later than StartTime.")]
        public DateTime EndTime { get; set; }
        public bool IsBooked { get; set; } = false; 

    }
}

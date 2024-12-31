using System.ComponentModel.DataAnnotations;

namespace TurboReserve.Models
{
    public class ScheduleSlot 
    {
        public int Id { get; set; }
        [Required]
        public int ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public bool IsBooked { get; set; } = false; 
    }

}

using System.ComponentModel.DataAnnotations;

namespace TurboReserve.Models
{
    public class ReservationViewModel
    {
        public int ReservationId { get; set; }
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public string ServiceProviderName { get; set; }
        [Required]
        public DateTime ReservationDate { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string CustomerName { get; set; }

    }

}

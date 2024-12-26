using Microsoft.AspNetCore.Identity;
using TurboReserve.Models.Enums;

namespace TurboReserve.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public IdentityUser Customer { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }

        public DateTime ReservationDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ReservationStatus Status { get; set; }
    }

}

using Microsoft.AspNetCore.Identity;

namespace TurboReserve.Models
{
    public class ServiceProvider
    {
        public int Id { get; set; }

        public string IdentityUserId { get; set; }

        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public virtual IdentityUser User { get; set; }

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}

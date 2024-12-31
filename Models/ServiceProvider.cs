using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TurboReserve.Models
{
    public class ServiceProvider
    {
        public int Id { get; set; }
        [Required]
        public string IdentityUserId { get; set; }
        [Required]
        public string BusinessName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public virtual IdentityUser User { get; set; }

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}

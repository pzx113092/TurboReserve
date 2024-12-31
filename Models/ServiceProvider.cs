using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TurboReserve.Models
{
    public class ServiceProvider
    {
        public int Id { get; set; }
        [Required]
        public required string IdentityUserId { get; set; }
        [Required]
        public required string BusinessName { get; set; }
        [Required]
        public required string Address { get; set; }
        [Required]
        public required string City { get; set; }
        [Required]
        public required string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public virtual IdentityUser User { get; set; }

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace TurboReserve.Models
{
    public class ServiceProviderViewModel
    {
        public int ServiceProviderId { get; set; }
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
        [Required]
        public List<ServiceViewModel> Services { get; set; }
    }

    public class ServiceViewModel
    {
        public int ServiceId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Price { get; set; }
    }
}

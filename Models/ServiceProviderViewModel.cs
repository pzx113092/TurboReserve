using System.Collections.Generic; // Dodano, aby List<> działało poprawnie
using System.ComponentModel.DataAnnotations;

namespace TurboReserve.Models
{
    public class ServiceProviderViewModel
    {
        public int ServiceProviderId { get; set; }
        public string BusinessName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<ServiceViewModel> Services { get; set; }
    }

    public class ServiceViewModel
    {
        public int ServiceId { get; set; } 

        
        public string Name { get; set; } 

        
        public string Description { get; set; } 

       
        public string Price { get; set; } 
    }
}

using Microsoft.AspNetCore.Identity;

namespace TurboReserve.Models
{
    public class CustomerProfile
    {
        public int Id { get; set; }

        public string IdentityUserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}

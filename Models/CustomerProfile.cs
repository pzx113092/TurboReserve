using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TurboReserve.Models
{
    public class CustomerProfile
    {
        public int Id { get; set; }
        [Required]
        public string IdentityUserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public virtual IdentityUser User { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
namespace TurboReserve.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nazwa jest wymagana.")]
        [StringLength(100, ErrorMessage = "Nazwa nie może przekraczać 100 znaków.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Opis jest wymagany.")]
        [StringLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Cena jest wymagana.")]
        [StringLength(50, ErrorMessage = "Cena nie może przekraczać 50 znaków.")]
        public string Price { get; set; }
        public int ServiceProviderId { get; set; }

        public ServiceProvider ServiceProvider { get; set; }

        public List<ScheduleSlot> ScheduleSlots { get; set; }

        public List<Reservation> Reservations { get; set; }
    }
}
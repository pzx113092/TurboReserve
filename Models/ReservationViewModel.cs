namespace TurboReserve.Models
{
    public class ReservationViewModel
    {
        public int ReservationId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceProviderName { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }

    }

}

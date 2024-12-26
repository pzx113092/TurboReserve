namespace TurboReserve.Models
{
    public class ServiceScheduleViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public List<ScheduleSlotViewModel> ScheduleSlots { get; set; }
    }

    public class ScheduleSlotViewModel
    {
        public int SlotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsBooked { get; set; } = false;
    }
}

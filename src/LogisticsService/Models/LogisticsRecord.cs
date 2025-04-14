// Models/LogisticsRecord.cs
namespace LogisticsService.Models
{
    public class LogisticsRecord
    {
        public Guid Id { get; set; }
        public string TruckId { get; set; } = default!;
        public string DriverId { get; set; } = default!;
        public string Origin { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public DateTime DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public string Status { get; set; } = "Scheduled"; // Scheduled, En Route, Delivered, Delayed
    }
}

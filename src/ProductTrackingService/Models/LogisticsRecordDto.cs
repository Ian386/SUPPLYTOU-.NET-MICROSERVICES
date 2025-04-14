// Models/LogisticsRecordDto.cs
namespace ProductTrackingService.Models
{
    public class LogisticsRecordDto
    {
        public string TruckId { get; set; } = "TBD"; // placeholder
        public string DriverId { get; set; } = "TBD"; // optional or fetched later
        public string Origin { get; set; } = default!;
        public string Destination { get; set; } = default!;
        public DateTime DepartureTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Scheduled";
    }
}

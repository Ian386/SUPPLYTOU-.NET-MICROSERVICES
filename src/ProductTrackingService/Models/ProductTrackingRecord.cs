// Models/ProductTrackingRecord.cs
namespace ProductTrackingService.Models
{
    public class ProductTrackingRecord
    {
        public Guid Id { get; set; }
        public string ProductId { get; set; } = default!;
        public string BatchNumber { get; set; } = default!;
        public string SourceFarmId { get; set; } = default!;
        public string DestinationRetailerId { get; set; } = default!;
        public string CurrentLocation { get; set; } = default!;
        public string Status { get; set; } = "In Transit"; // In Transit, Delivered, Delayed, Returned
        public DateTime ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}

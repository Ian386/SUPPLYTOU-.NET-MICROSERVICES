namespace FarmService.Models
{
    public class ProductTrackingRecordDto
    {
        public Guid ProductId { get; set; }
        public string BatchNumber { get; set; } = "Initial-Batch";
        public Guid SourceFarmId { get; set; }
        public Guid DestinationRetailerId { get; set; } // You can set a dummy for now
        public string CurrentLocation { get; set; } = "Farm";
        public string Status { get; set; } = "Pending";
        public DateTime ShippedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; } = null;
    }
}

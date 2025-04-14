using ProductTrackingService.Data;
using ProductTrackingService.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace ProductTrackingService.Services
{
    public class ProductTrackingDataService
    {
        private readonly ProductTrackingDbContext _context;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ProductTrackingDataService(ProductTrackingDbContext context, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _config = config;
            _httpClient = httpClientFactory.CreateClient("LogisticsClient");
        }

        public IEnumerable<ProductTrackingRecord> GetAll() => _context.ProductTrackingRecords.ToList();

        public ProductTrackingRecord? GetById(Guid id) => _context.ProductTrackingRecords.FirstOrDefault(r => r.Id == id);

        public ProductTrackingRecord Add(ProductTrackingRecord record)
        {
            _context.ProductTrackingRecords.Add(record);
            _context.SaveChanges();
            return record;
        }

        public bool Update(Guid id, ProductTrackingRecord updated)
        {
            var existing = _context.ProductTrackingRecords.FirstOrDefault(r => r.Id == id);
            if (existing is null) return false;

            existing.ProductId = updated.ProductId;
            existing.BatchNumber = updated.BatchNumber;
            existing.SourceFarmId = updated.SourceFarmId;
            existing.DestinationRetailerId = updated.DestinationRetailerId;
            existing.CurrentLocation = updated.CurrentLocation;
            existing.Status = updated.Status;
            existing.ShippedAt = updated.ShippedAt;
            existing.DeliveredAt = updated.DeliveredAt;

            _context.SaveChanges();

            // 🔔 If the status is ReadyToShip, notify LogisticsService
            if (updated.Status == "ReadyToShip")
            {
                NotifyLogisticsService(existing);
            }

            return true;
        }

        private async void NotifyLogisticsService(ProductTrackingRecord record)
        {
            var baseUrl = _config["LogisticsService:BaseUrl"];
            var token = _config["LogisticsService:AuthToken"];

            var logisticsRecord = new
            {
                TruckId = "TRUCK-123",
                DriverId = "system-auto",
                Origin = record.CurrentLocation,
                Destination = record.DestinationRetailerId,
                DepartureTime = DateTime.UtcNow,
                Status = "Scheduled"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/Logistics")
            {
                Content = JsonContent.Create(logisticsRecord)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to notify LogisticsService: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling LogisticsService: {ex.Message}");
            }
        }



        public bool Delete(Guid id)
        {
            var record = _context.ProductTrackingRecords.FirstOrDefault(r => r.Id == id);
            if (record is null) return false;

            _context.ProductTrackingRecords.Remove(record);
            _context.SaveChanges();
            return true;
        }
    }
}

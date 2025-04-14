using FarmService.Models;
using FarmService.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FarmService.Services
{
    public class FarmDataService(
        FarmDbContext context,
        HttpClient httpClient,
        ILogger<FarmDataService> logger,
        IConfiguration config)
    {
        private readonly FarmDbContext _context = context;
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<FarmDataService> _logger = logger;
        private readonly string _jwtToken = config["ApiSettings:ProductTrackingJwtToken"]
    ?? throw new ArgumentNullException("ProductTracking JWT token not configured");


        public async Task<IEnumerable<Farm>> GetAllAsync() =>
            await _context.Farms.ToListAsync();

        public async Task<Farm?> GetByIdAsync(Guid id) =>
            await _context.Farms.FindAsync(id);

        public async Task<Farm> AddAsync(Farm farm)
        {
            farm.Id = Guid.NewGuid();
            _context.Farms.Add(farm);
            await _context.SaveChangesAsync();

            await NotifyProductTrackingServiceAsync(farm);
            return farm;
        }

        private async Task NotifyProductTrackingServiceAsync(Farm farm)
        {
            _logger.LogDebug("Calling NotifyProductTrackingServiceAsync at {Time}", DateTime.UtcNow);
            var trackingRecord = new
            {
                ProductId = Guid.NewGuid(),
                BatchNumber = "Batch-001",
                SourceFarmId = farm.Id,
                DestinationRetailerId = Guid.NewGuid(),
                CurrentLocation = farm.Location,
                Status = "Created",
                ShippedAt = DateTime.UtcNow,
                DeliveredAt = (DateTime?)null
            };

            var json = JsonSerializer.Serialize(trackingRecord);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5250/api/ProductTracking")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            try
            {
                _logger.LogInformation("Notifying ProductTrackingService with tracking record: {TrackingRecord}", trackingRecord);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully notified ProductTrackingService.");
                }
                else
                {
                    _logger.LogWarning("Failed to create tracking record: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling ProductTrackingService");
            }
        }



        public async Task<bool> UpdateAsync(Guid id, Farm updatedFarm)
        {
            var existing = await _context.Farms.FindAsync(id);
            if (existing == null) return false;

            existing.Name = updatedFarm.Name;
            existing.OwnerId = updatedFarm.OwnerId;
            existing.Location = updatedFarm.Location;
            existing.SizeInAcres = updatedFarm.SizeInAcres;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _context.Farms.FindAsync(id);
            if (existing == null) return false;

            _context.Farms.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class ApiSettings
    {
        public string ProductTrackingBaseUrl { get; set; } = string.Empty;
        public string ProductTrackingJwtToken { get; set; } = string.Empty;
    }
}

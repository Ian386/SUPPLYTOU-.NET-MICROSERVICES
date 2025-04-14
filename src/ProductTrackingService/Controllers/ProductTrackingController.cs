using Microsoft.AspNetCore.Mvc;
using ProductTrackingService.Models;
using ProductTrackingService.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;

namespace ProductTrackingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductTrackingController : ControllerBase
    {
        private readonly ProductTrackingDataService _service;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductTrackingController(ProductTrackingDataService service, IHttpClientFactory httpClientFactory)
        {
            _service = service;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Retailer")]
        public IActionResult GetById(Guid id)
        {
            var record = _service.GetById(id);
            return record is null ? NotFound() : Ok(record);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Driver")]
        public IActionResult Create(ProductTrackingRecord record)
        {
            Console.WriteLine($"Received tracking record: {record.SourceFarmId}"); // Add this
            var created = _service.Add(record);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> Update(Guid id, ProductTrackingRecord record)
        {
            var existing = _service.GetById(id);
            if (existing is null) return NotFound();

            var previousStatus = existing.Status;

            var success = _service.Update(id, record);
            if (!success) return NotFound();

            // Trigger Logistics creation if marked as ReadyToShip
            if (record.Status == "ReadyToShip" && previousStatus != "ReadyToShip")
            {
                var logisticsDto = new LogisticsRecordDto
                {
                    Origin = record.CurrentLocation,
                    Destination = "Retailer: " + record.DestinationRetailerId,
                };

                try
                {
                    var client = _httpClientFactory.CreateClient("LogisticsClient");

                    // Optional: Add token if securing between services
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-token");

                    var response = await client.PostAsJsonAsync("/api/Logistics", logisticsDto);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to notify LogisticsService: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LogisticsService error: {ex.Message}");
                }
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            var success = _service.Delete(id);
            return success ? NoContent() : NotFound();
        }
    }
}

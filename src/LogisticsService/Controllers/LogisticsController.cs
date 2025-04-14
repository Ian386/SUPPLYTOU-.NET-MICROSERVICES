// LogisticsService/Controllers/LogisticsController.cs
using LogisticsService.Models;
using LogisticsService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LogisticsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogisticsController : ControllerBase
    {
        private readonly LogisticsDataService _logisticsService;

        public LogisticsController(LogisticsDataService logisticsService)
        {
            _logisticsService = logisticsService;
        }

        private string GetUserIdFromToken()
        {
            return User.FindFirst("sub")?.Value ?? string.Empty;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Driver")]
        public IActionResult GetAll()
        {
            var userId = GetUserIdFromToken();
            if (User.IsInRole("Admin"))
            {
                return Ok(_logisticsService.GetAll());
            }
            else
            {
                var filtered = _logisticsService.GetAll()
                    .Where(r => r.DriverId == userId);
                return Ok(filtered);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Driver")]
        public IActionResult GetById(Guid id)
        {
            var record = _logisticsService.GetById(id);
            if (record is null)
                return NotFound();

            var userId = GetUserIdFromToken();
            if (User.IsInRole("Admin") || record.DriverId == userId)
                return Ok(record);

            return Forbid();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Driver")]
        public IActionResult Create(LogisticsRecord record)
        {
            // Set DriverId from token if role is Driver
            if (User.IsInRole("Driver"))
            {
                record.DriverId = GetUserIdFromToken();
            }

            var created = _logisticsService.Add(record);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Driver")]
        public IActionResult Update(Guid id, LogisticsRecord record)
        {
            var existing = _logisticsService.GetById(id);
            if (existing is null)
                return NotFound();

            var userId = GetUserIdFromToken();
            if (!User.IsInRole("Admin") && existing.DriverId != userId)
                return Forbid();

            var success = _logisticsService.Update(id, record);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            var success = _logisticsService.Delete(id);
            return success ? NoContent() : NotFound();
        }
    }
}

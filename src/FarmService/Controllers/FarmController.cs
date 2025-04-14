// Controllers/FarmController.cs
using Microsoft.AspNetCore.Mvc;
using FarmService.Models;
using FarmService.Services;
using Microsoft.AspNetCore.Authorization;

namespace FarmService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FarmController : ControllerBase
    {
        private readonly FarmDataService _service;
        private readonly IHttpClientFactory _httpClientFactory;

        public FarmController(FarmDataService service, IHttpClientFactory httpClientFactory)
        {
            _service = service;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var farm = await _service.GetByIdAsync(id);
            if (farm == null) return NotFound();

            var userId = GetUserIdFromToken();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && farm.OwnerId != userId)
                return Forbid("You are not authorized to view this farm.");

            return Ok(farm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Create(Farm farm)
        {
            var userId = GetUserIdFromToken();
            farm.OwnerId = userId ?? throw new InvalidOperationException("User ID not found in token");

            var created = await _service.AddAsync(farm); // Let _service handle everything

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Update(Guid id, Farm farm)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing is null) return NotFound();

            var userId = GetUserIdFromToken();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && existing.OwnerId != userId)
                return Forbid();

            // Optionally enforce immutability on CreatedBy
            farm.Id = id;
            farm.OwnerId = existing.OwnerId; // Preserve the original owner
            var updated = await _service.UpdateAsync(id, farm);

            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Farmer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing is null) return NotFound();

            var userId = GetUserIdFromToken();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && existing.OwnerId != userId)
                return Forbid();

            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        private string? GetUserIdFromToken()
        {
            // Tries to get user ID from "sub" or "nameid" claim
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type.EndsWith("nameidentifier"))?.Value;
            return userId;
        }

    }

}

// LogisticsService/Services/LogisticsDataService.cs
using LogisticsService.Data;
using LogisticsService.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticsService.Services
{
    public class LogisticsDataService
    {
        private readonly LogisticsDbContext _context;

        public LogisticsDataService(LogisticsDbContext context)
        {
            _context = context;
        }

        // Get all records
        public IEnumerable<LogisticsRecord> GetAll()
        {
            return _context.LogisticsRecords.ToList();
        }

        // Get record by ID
        public LogisticsRecord? GetById(Guid id)
        {
            return _context.LogisticsRecords.FirstOrDefault(r => r.Id == id);
        }

        // Add a new record
        public LogisticsRecord Add(LogisticsRecord record)
        {
            _context.LogisticsRecords.Add(record);
            _context.SaveChanges();
            return record;
        }

        // Update an existing record
        public bool Update(Guid id, LogisticsRecord updatedRecord)
        {
            var existingRecord = _context.LogisticsRecords.FirstOrDefault(r => r.Id == id);
            if (existingRecord is null)
                return false;

            existingRecord.TruckId = updatedRecord.TruckId;
            existingRecord.DriverId = updatedRecord.DriverId;
            existingRecord.Origin = updatedRecord.Origin;
            existingRecord.Destination = updatedRecord.Destination;
            existingRecord.DepartureTime = updatedRecord.DepartureTime;
            existingRecord.ArrivalTime = updatedRecord.ArrivalTime;
            existingRecord.Status = updatedRecord.Status;

            _context.SaveChanges();
            return true;
        }

        // Delete a record
        public bool Delete(Guid id)
        {
            var record = _context.LogisticsRecords.FirstOrDefault(r => r.Id == id);
            if (record is null)
                return false;

            _context.LogisticsRecords.Remove(record);
            _context.SaveChanges();
            return true;
        }
    }
}

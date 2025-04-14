// LogisticsService/Data/LogisticsDbContext.cs
using Microsoft.EntityFrameworkCore;
using LogisticsService.Models;

namespace LogisticsService.Data
{
    public class LogisticsDbContext : DbContext
    {
        public LogisticsDbContext(DbContextOptions<LogisticsDbContext> options)
            : base(options)
        {
        }

        public DbSet<LogisticsRecord> LogisticsRecords => Set<LogisticsRecord>();
    }
}

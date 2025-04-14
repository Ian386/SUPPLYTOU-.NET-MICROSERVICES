using Microsoft.EntityFrameworkCore;
using ProductTrackingService.Models;

namespace ProductTrackingService.Data
{
    public class ProductTrackingDbContext : DbContext
    {
        public ProductTrackingDbContext(DbContextOptions<ProductTrackingDbContext> options)
            : base(options) { }

        public DbSet<ProductTrackingRecord> ProductTrackingRecords => Set<ProductTrackingRecord>();
    }
}

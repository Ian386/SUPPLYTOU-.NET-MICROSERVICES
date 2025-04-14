// Data/FarmDbContext.cs
using Microsoft.EntityFrameworkCore;
using FarmService.Models;

namespace FarmService.Data
{
    public class FarmDbContext : DbContext
    {
        public FarmDbContext(DbContextOptions<FarmDbContext> options) : base(options) { }

        public DbSet<Farm> Farms => Set<Farm>();
    }
}

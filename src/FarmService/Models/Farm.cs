// Models/Farm.cs
namespace FarmService.Models
{
    public class Farm
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string OwnerId { get; set; } = default!;
        public string Location { get; set; } = default!;
        public double SizeInAcres { get; set; }
    }
}

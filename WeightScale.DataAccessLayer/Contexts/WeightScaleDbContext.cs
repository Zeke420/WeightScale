using System.Data.Entity;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.DataAccessLayer.Contexts
{
    public class WeightScaleDbContext : DbContext
    {
        public WeightScaleDbContext() : base("WeightScaleDbContext")
        {
        }
        
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
    }
}
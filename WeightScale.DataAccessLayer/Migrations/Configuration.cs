using System.Data.Entity.Migrations;
using WeightScale.DataAccessLayer.Contexts;

namespace WeightScale.DataAccessLayer.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<WeightScaleDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WeightScaleDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
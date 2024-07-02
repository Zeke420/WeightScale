using WeightScale.DataAccessLayer.Contexts;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.DataAccessLayer.Repository.Implementation
{
    public class PackageRepository : IPackageRepository
    {
        private readonly WeightScaleDbContext _dbContext;

        public PackageRepository(WeightScaleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Package package)
        {
            using (var dbContext = new WeightScaleDbContext())
            { 
                dbContext.Packages.Add(package);
                dbContext.SaveChanges();
            }
        }
    }
}
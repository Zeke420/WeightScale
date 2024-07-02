using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.DataAccessLayer.Repository
{
    public interface IPackageRepository
    {
        void Add(Package package);
    }
}
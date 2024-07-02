using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using WeightScale.DataAccessLayer.Contexts;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.DataAccessLayer.Repository.Implementation
{
    public class CouriersRepository : ICouriersRepository
    {
        private readonly WeightScaleDbContext _dbContext;

        public CouriersRepository(WeightScaleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddCourier(Courier courier)
        {
            _dbContext.Couriers.Add(courier);
            _dbContext.SaveChanges();
        }

        public void UpdateCourier(Courier courier)
        {
            _dbContext.Couriers.AddOrUpdate(courier);
            _dbContext.SaveChanges();
        }

        public void DeleteCourier(Courier courier)
        {
            _dbContext.Couriers.Remove(courier);
            _dbContext.SaveChanges();
        }

        public List<Courier> GetCouriers()
        {
            return _dbContext.Couriers.ToList();
        }
    }
}
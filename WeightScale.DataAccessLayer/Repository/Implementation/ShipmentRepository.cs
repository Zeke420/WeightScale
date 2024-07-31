using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using WeightScale.DataAccessLayer.Contexts;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.DataAccessLayer.Repository.Implementation
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly WeightScaleDbContext _dbContext;

        public ShipmentRepository(WeightScaleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Shipment GetFirstUnFinishedShipment()
        {
            using (var dbContext = new WeightScaleDbContext())
            {
                return dbContext.Shipments
                                .Where(s => !s.IsFinished)
                                .Include(x => x.Packages)
                                .First();
            }
        }

        public void AddShipment(Shipment shipment)
        {
            _dbContext.Shipments.Add(shipment);
            _dbContext.SaveChanges();
        }

        public void UpdateShipment(Shipment shipment)
        {
            _dbContext.Shipments.AddOrUpdate(shipment);
            _dbContext.SaveChanges();
        }

        public List<Shipment> GetShipmentsByDate(DateTime date)
        {
            return _dbContext.Shipments
                             .Where(s => s.ShipmentDate == date)
                             .Include(s => s.Courier)
                             .ToList();
        }

        public List<Shipment> GetShipmentsWeightsByDate(DateTime date)
        {
            return _dbContext.Shipments
                             .Where(x => x.ShipmentDate == date)
                             .Include(x => x.Packages)
                             .Include(x => x.Courier)
                             .ToList();
        }

        public void CompleteShipment(Shipment shipment)
        {
            _dbContext.Shipments.AddOrUpdate(shipment);
            _dbContext.SaveChanges();
        }

        public void Delete(Shipment shipment)
        {
            _dbContext.Shipments.Remove(shipment);
            _dbContext.SaveChanges();
        }

        public List<Shipment> GetShipmentsInRange(DateTime startDate, DateTime endDate, List<Courier> couriers)
        {
            var courierIds = couriers.Select(c => c.Id)
                                     .ToList();

            var query = _dbContext.Shipments
                                  .Where(x => x.ShipmentDate >= startDate && x.ShipmentDate <= endDate)
                                  .Include(x => x.Packages)
                                  .Include(x => x.Courier);

            if (courierIds.Any())
            {
                query = query.Where(x => courierIds.Contains(x.Courier.Id));
            }

            return query.ToList();
        }
    }
}

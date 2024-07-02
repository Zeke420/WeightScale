using System;
using System.Collections.Generic;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.DataAccessLayer.Repository
{
    public interface IShipmentRepository
    {
        Shipment GetFirstUnFinishedShipment();
        void AddShipment(Shipment shipment);
        void UpdateShipment(Shipment shipment);
        List<Shipment> GetShipmentsByDate(DateTime date);
        List<Shipment> GetShipmentsWeightsByDate(DateTime date);
        void CompleteShipment(Shipment shipment);
        void Delete(Shipment shipment);
        List<Shipment> GetShipmentsInRange(DateTime startDate, DateTime endDate, List<Courier> couriers);
    }
}
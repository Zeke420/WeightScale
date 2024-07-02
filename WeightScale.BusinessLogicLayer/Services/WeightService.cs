using System;
using System.Collections.Generic;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IWeightService
    {
        List<Shipment> GetShipmentWeightByDate(DateTime date);
        void CompleteShipment(Shipment shipment);
    }

    public class WeightService : IWeightService
    {
        private readonly IShipmentService _shipmentService;
        
        public WeightService(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }
        
        public List<Shipment> GetShipmentWeightByDate(DateTime date)
        {
            return _shipmentService.GetShipmentWeightByDate(date);
        }

        public void CompleteShipment(Shipment shipment)
        {
            _shipmentService.CompleteShipment(shipment);
        }
    }
}
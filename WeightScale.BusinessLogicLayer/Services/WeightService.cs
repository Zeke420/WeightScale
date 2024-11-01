﻿using System;
using System.Collections.Generic;
using WeightScale.BusinessLogicLayer.Mappers;
using WeightScale.BusinessLogicLayer.Models;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IWeightService
    {
        List<ShipmentModel> GetShipmentWeightByDate(DateTime date);
        void CompleteShipment(ShipmentModel shipmentModel);
    }

    public class WeightService : IWeightService
    {
        private readonly IShipmentService _shipmentService;

        public WeightService(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        public List<ShipmentModel> GetShipmentWeightByDate(DateTime date)
        {
            var shipments = _shipmentService.GetShipmentWeightByDate(date);
            var shipmentModels = ShipmentMapper.Map(shipments);
            return shipmentModels;
        }

        public void CompleteShipment(ShipmentModel shipmentModel)
        {
            var shipment = ShipmentMapper.MapToEntity(shipmentModel);
            _shipmentService.CompleteShipment(shipment);
        }
    }
}

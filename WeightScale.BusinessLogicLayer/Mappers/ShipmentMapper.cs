using System.Collections.Generic;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Mappers
{
    public static class ShipmentMapper
    {
        public static List<ShipmentModel> Map(List<Shipment> shipments)
        {
            return shipments
                .ConvertAll(shipment => new ShipmentModel
                {
                    Id = shipment.Id,
                    ShipmentDate = shipment.ShipmentDate,
                    CourierId = shipment.CourierId,
                    Courier = shipment.Courier,
                    IsFinished = shipment.IsFinished,
                    Packages = PackageMapper.Map(shipment.Packages)
                });
        }

        public static List<Shipment> MapToEntity(List<ShipmentModel> shipmentModels)
        {
            return shipmentModels
                .ConvertAll(shipmentModel => new Shipment
                {
                    Id = shipmentModel.Id,
                    ShipmentDate = shipmentModel.ShipmentDate,
                    CourierId = shipmentModel.CourierId,
                    Courier = shipmentModel.Courier,
                    IsFinished = shipmentModel.IsFinished,
                    Packages = PackageMapper.MapToEntity(shipmentModel.Packages)
                });
        }

        public static Shipment MapToEntity(ShipmentModel shipmentModel)
        {
            return new Shipment
            {
                Id = shipmentModel.Id,
                ShipmentDate = shipmentModel.ShipmentDate,
                CourierId = shipmentModel.CourierId,
                Courier = shipmentModel.Courier,
                IsFinished = shipmentModel.IsFinished,
                Packages = PackageMapper.MapToEntity(shipmentModel.Packages)
            };
        }
    }
}

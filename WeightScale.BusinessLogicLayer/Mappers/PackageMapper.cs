using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Mappers
{
    public static class PackageMapper
    {
        public static List<PackageModel> Map(List<Package> packages, List<PackageMoveModel> packageMoveModels)
        {
            if (packages == null)
            {
                return new List<PackageModel>();
            }

            return packages
               .ConvertAll(package => new PackageModel
                                      {
                                          Id = package.Id,
                                          EmptyWeight = package.EmptyWeight?.ToString("F1"),
                                          FullWeight = package.FullWeight?.ToString("F1"),
                                          WeightDifference = (package.FullWeight - package.EmptyWeight)?.ToString("F1"),
                                          ShipmentId = package.ShipmentId,
                                          PackageMoves = new ObservableCollection<PackageMoveModel>(packageMoveModels)
                                      });
        }

        public static PackageModel Map(Package package, List<ShipmentModel> shipmentModels)
        {
            var shipment = shipmentModels.FirstOrDefault(x => x.Id == package.ShipmentId);

            return new PackageModel
                   {
                       Id = package.Id,
                       EmptyWeight = package.EmptyWeight?.ToString("F1"),
                       FullWeight = package.FullWeight?.ToString("F1"),
                       WeightDifference = (package.FullWeight - package.EmptyWeight)?.ToString("F1"),
                       ShipmentId = package.ShipmentId,
                       PackageMoves = new ObservableCollection<PackageMoveModel>(shipmentModels.Where(x => x != shipment)
                                                    .Select(x => new PackageMoveModel
                                                                 {
                                                                     CourierName = x.Courier.Name,
                                                                     ShipmentId = x.Id
                                                                 })
                                                    .ToList())
                   };
        }

        public static List<Package> MapToEntity(List<PackageModel> packageModels)
        {
            return packageModels
               .ConvertAll(packageModel => new Package
                                           {
                                               Id = packageModel.Id,
                                               EmptyWeight = double.Parse(packageModel.EmptyWeight),
                                               FullWeight = double.Parse(packageModel.FullWeight),
                                               ShipmentId = packageModel.ShipmentId
                                           });
        }

        public static Package MapToEntity(PackageModel packageModel)
        {
            return new Package
                   {
                       Id = packageModel.Id,
                       EmptyWeight = double.Parse(packageModel.EmptyWeight),
                       FullWeight = double.Parse(packageModel.FullWeight),
                       ShipmentId = packageModel.ShipmentId
                   };
        }
    }
}

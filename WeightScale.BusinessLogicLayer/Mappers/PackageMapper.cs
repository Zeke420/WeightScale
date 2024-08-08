using System.Collections.Generic;
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
                                                   WeightDifference = (package.FullWeight- package.EmptyWeight)?.ToString("F1"),
                                                   ShipmentId = package.ShipmentId,
                                                   PackageMoves = packageMoveModels
                                           });
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

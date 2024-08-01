using System.Collections.Generic;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Mappers
{
    public static class PackageMapper
    {
        public static List<PackageModel> Map(List<Package> packages)
        {
            if (packages == null)
            {
                return new List<PackageModel>();
            }

            return packages
                    .ConvertAll(package => new PackageModel
                                           {
                                                   Id = package.Id,
                                                   EmptyWeight = package.EmptyWeight,
                                                   FullWeight = package.FullWeight,
                                                   ShipmentId = package.ShipmentId
                                           });
        }

        public static List<Package> MapToEntity(List<PackageModel> packageModels)
        {
            return packageModels
                    .ConvertAll(packageModel => new Package
                                                {
                                                        Id = packageModel.Id,
                                                        EmptyWeight = packageModel.EmptyWeight,
                                                        FullWeight = packageModel.FullWeight,
                                                        ShipmentId = packageModel.ShipmentId
                                                });
        }

        public static Package MapToEntity(PackageModel packageModel)
        {
            return new Package
                   {
                           Id = packageModel.Id,
                           EmptyWeight = packageModel.EmptyWeight,
                           FullWeight = packageModel.FullWeight,
                           ShipmentId = packageModel.ShipmentId
                   };
        }
    }
}
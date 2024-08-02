using System.Collections.Generic;
using System.Linq;
using WeightScale.BusinessLogicLayer.Models.CSV;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Mappers
{
    public static class ExportFileMapper
    {
        public static List<ExportModel> Map(List<Shipment> shipments)
        {
            var exportModels = new List<ExportModel>();
            const string packageHeader =
                    "Container,Date,Weight with Birds (kg),Weight without Birds (kg),Weight of Birds (kg)";

            foreach (var shipment in shipments)
            {
                var packages = new List<string>();
                var exportModel = new ExportModel
                                  {
                                          ShipmentInfo = $"Shipment ID,{shipment.Id},Courier Name,{shipment.Courier.Name}",
                                          ShipmentDate = shipment.ShipmentDate
                                  };

                if (shipment.Packages == null || !shipment.Packages.Any())
                {
                    continue;
                }

                exportModel.PackageHeader = packageHeader;
                var index = 1;
                foreach (var package in shipment.Packages)
                {
                    packages.Add($"{index},{shipment.ShipmentDate:dd-MM-yy},{package.FullWeight},{package.EmptyWeight},{package.FullWeight - package.EmptyWeight}");
                    index++;
                }

                exportModel.Packages = packages;
                exportModel.PackageTotals =
                        $",Total Weight,{shipment.Packages.Sum(p => p.FullWeight)},{shipment.Packages.Sum(p => p.EmptyWeight)},{shipment.Packages.Sum(p => p.FullWeight - p.EmptyWeight)}";

                exportModels.Add(exportModel);
            }

            return exportModels;
        }
    }
}

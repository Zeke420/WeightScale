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

            foreach (var shipment in shipments)
            {
                var packageFullTotals = shipment.Packages.Sum(p => p.FullWeight);
                var packageEmptyTotals = shipment.Packages.Sum(p => p.EmptyWeight);
                var packageNetTotals = shipment.Packages.Sum(p => p.FullWeight - p.EmptyWeight);
                var exportModel = new ExportModel
                                  {
                                      Shipment = shipment,
                                      PackageFullTotals = packageFullTotals?.ToString("F1"),
                                      PackageEmptyTotals = packageEmptyTotals?.ToString("F1"),
                                      PackageNetTotals = packageNetTotals?.ToString("F1")
                                  };
                exportModels.Add(exportModel);
            }

            return exportModels;
        }
    }
}

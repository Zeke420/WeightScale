using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IFileExportService
    {
        void ExportToCsv(List<Shipment> shipments);
    }

    public class FileExportService : IFileExportService
    {
        private readonly ApplicationSettings _applicationSettings;

        public FileExportService(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings.Value;
        }

        public void ExportToCsv(List<Shipment> shipments)
        {
            var csv = new StringBuilder();
            var shipmentHeader = "Shipment ID,Shipment Date,Courier Name,Is Finished";
            var packageHeader = "    Package ID,Full Weight,Empty Weight";

            foreach (var shipment in shipments)
            {
                csv.AppendLine(shipmentHeader);
                csv.AppendLine($"{shipment.Id},{shipment.ShipmentDate},{shipment.Courier.Name},{shipment.IsFinished}");
                if (shipment.Packages == null || !shipment.Packages.Any())
                {
                    continue;
                }

                csv.AppendLine(packageHeader);
                foreach (var package in shipment.Packages)
                {
                    csv.AppendLine($"    {package.Id},{package.FullWeight},{package.EmptyWeight}");
                }
            }

            var startDateFormat = shipments.First()
                                           .ShipmentDate.ToUniversalTime()
                                           .ToString("yyyyMMdd");
            var endDateFormat = shipments.Last()
                                         .ShipmentDate.ToUniversalTime()
                                         .ToString("yyyyMMdd");
            var fileName = $"Report_{startDateFormat}_{endDateFormat}.csv";
            var filePath = Path.Combine(_applicationSettings.ReportFilePath, fileName); // Use Path.Combine for safety

            File.WriteAllText(filePath, csv.ToString());
        }
    }
}
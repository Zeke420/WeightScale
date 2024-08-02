using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.BusinessLogicLayer.Models.CSV;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IFileExportService
    {
        void ExportToCsv(List<ExportModel> exportModels);
    }

    public class FileExportService : IFileExportService
    {
        private readonly ApplicationSettings _applicationSettings;

        public FileExportService(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings.Value;
        }

        public void ExportToCsv(List<ExportModel> exportModels)
        {
            var csv = new StringBuilder();

            foreach (var exportModel in exportModels)
            {
                csv.AppendLine(exportModel.ShipmentInfo);
                csv.AppendLine();

                if (exportModel.Packages == null || !exportModel.Packages.Any())
                {
                    continue;
                }

                csv.AppendLine(exportModel.PackageHeader);
                csv.AppendLine(exportModel.Packages.Aggregate((i, j) => i + "\n" + j));
                csv.AppendLine(exportModel.PackageTotals);
                csv.AppendLine();
            }

            var startDateFormat = exportModels.First()
                                              .ShipmentDate.ToUniversalTime()
                                              .ToString("yyyyMMdd");

            var endDateFormat = exportModels.Last()
                                            .ShipmentDate.ToUniversalTime()
                                            .ToString("yyyyMMdd");

            var fileName = $"Report_{startDateFormat}_{endDateFormat}.csv";
            var filePath = Path.Combine(_applicationSettings.ReportFilePath, fileName);

            var directoryPath = Path.GetDirectoryName(filePath);
            if(!Directory.Exists(directoryPath))
            {
                if (directoryPath != null)
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }

            File.WriteAllText(filePath, csv.ToString());
        }
    }
}

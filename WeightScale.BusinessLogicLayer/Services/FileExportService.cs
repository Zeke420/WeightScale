using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WeightScale.BusinessLogicLayer.Models;
using WeightScale.BusinessLogicLayer.Models.CSV;

namespace WeightScale.BusinessLogicLayer.Services
{
    public interface IFileExportService
    {
        void ExportToExcel(List<ExportModel> exportModels);
    }

    public class FileExportService : IFileExportService
    {
        private readonly ApplicationSettings _applicationSettings;

        public FileExportService(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings.Value;
        }

        public void ExportToExcel(List<ExportModel> exportModels)
        {
            var startDateFormat = exportModels.First()
                                              .Shipment.ShipmentDate.ToUniversalTime()
                                              .ToString("yyyyMMdd");

            var endDateFormat = exportModels.Last()
                                            .Shipment.ShipmentDate.ToUniversalTime()
                                            .ToString("yyyyMMdd");

            var fileName = $"Report_{startDateFormat}_{endDateFormat}.xlsx";
            var filePath = Path.Combine(_applicationSettings.ReportFilePath, fileName);

            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                if (directoryPath != null)
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet("Report");

                var row = 0;
                foreach (var exportModel in exportModels)
                {
                    IRow dataRow = sheet.CreateRow(row++);
                    dataRow.CreateCell(0)
                           .SetCellValue("Shipment Id");
                    dataRow.CreateCell(1)
                           .SetCellValue(exportModel.Shipment.Id);
                    dataRow.CreateCell(2)
                           .SetCellValue("Courier name");
                    dataRow.CreateCell(3)
                           .SetCellValue(exportModel.Shipment.Courier.Name);

                    if (exportModel.Shipment.Packages == null
                        || !exportModel.Shipment.Packages.Any())
                    {
                        continue;
                    }

                    dataRow = sheet.CreateRow(row++);
                    var containerCount = 1;
                    dataRow.CreateCell(0)
                           .SetCellValue("Container");
                    dataRow.CreateCell(1)
                           .SetCellValue("Date");
                    dataRow.CreateCell(2)
                           .SetCellValue("Weight with Birds (kg)");
                    dataRow.CreateCell(3)
                           .SetCellValue("Weight without Birds (kg)");
                    dataRow.CreateCell(4)
                           .SetCellValue("Weight of Birds (kg)");

                    foreach (var package in exportModel.Shipment.Packages)
                    {
                        dataRow = sheet.CreateRow(row++);
                        dataRow.CreateCell(0)
                               .SetCellValue(containerCount);
                        dataRow.CreateCell(1)
                               .SetCellValue(exportModel.Shipment.ShipmentDate.ToString("dd-MM-yy"));
                        dataRow.CreateCell(2)
                               .SetCellValue(package.FullWeight?.ToString("F1"));
                        dataRow.CreateCell(3)
                               .SetCellValue(package.EmptyWeight?.ToString("F1"));
                        dataRow.CreateCell(4)
                               .SetCellValue((package.FullWeight - package.EmptyWeight)?.ToString("F1"));

                        containerCount++;
                    }

                    dataRow = sheet.CreateRow(row++);
                    dataRow.CreateCell(0)
                           .SetCellValue(string.Empty);
                    dataRow.CreateCell(1)
                           .SetCellValue(string.Empty);
                    dataRow.CreateCell(2)
                           .SetCellValue(exportModel.PackageFullTotals);
                    dataRow.CreateCell(3)
                           .SetCellValue(exportModel.PackageEmptyTotals);
                    dataRow.CreateCell(4)
                           .SetCellValue(exportModel.PackageNetTotals);

                    sheet.CreateRow(row++);
                }

                workbook.Write(fs);
            }
        }
    }
}

using System;
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

                var numericCellStyle = CreateNumericCellStyle(workbook);
                var dateCellStyle = CreateDateTimeCellStyle(workbook);

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
                           .SetCellValue("Full Date");
                    dataRow.CreateCell(2)
                           .SetCellValue("Weight with Birds (kg)");
                    dataRow.CreateCell(3)
                           .SetCellValue("Empty Date");
                    dataRow.CreateCell(4)
                           .SetCellValue("Weight without Birds (kg)");
                    dataRow.CreateCell(5)
                           .SetCellValue("Weight of Birds (kg)");

                    foreach (var package in exportModel.Shipment.Packages)
                    {
                        dataRow = sheet.CreateRow(row++);
                        dataRow.CreateCell(0)
                               .SetCellValue(containerCount);

                        CreateDateCell(dataRow, 1, package.FullPackageDate, dateCellStyle);
                        CreateNumericCell(dataRow, 2, package.FullWeight, numericCellStyle);
                        CreateDateCell(dataRow, 3, package.EmptyPackageDate, dateCellStyle);
                        CreateNumericCell(dataRow, 4, package.EmptyWeight, numericCellStyle);
                        CreateNumericCell(dataRow, 5, package.FullWeight - package.EmptyWeight, numericCellStyle);

                        containerCount++;
                    }

                    dataRow = sheet.CreateRow(row++);
                    dataRow.CreateCell(0)
                           .SetCellValue(string.Empty);
                    dataRow.CreateCell(1)
                           .SetCellValue(string.Empty);
                    CreateNumericCell(dataRow, 2, exportModel.PackageFullTotals, numericCellStyle);
                    dataRow.CreateCell(3)
                           .SetCellValue(string.Empty);
                    CreateNumericCell(dataRow, 4, exportModel.PackageEmptyTotals, numericCellStyle);
                    CreateNumericCell(dataRow, 5, exportModel.PackageNetTotals, numericCellStyle);

                    sheet.CreateRow(row++);
                }

                workbook.Write(fs);
            }
        }

        private static ICellStyle CreateNumericCellStyle(IWorkbook workbook)
        {
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("0.00");
            return cellStyle;
        }

        private static void CreateNumericCell(IRow dataRow, int rowIndex, double? value, ICellStyle cellStyle)
        {
            var cell = dataRow.CreateCell(rowIndex);
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(value ?? 0);
            cell.CellStyle = cellStyle;
        }

        private static ICellStyle CreateDateTimeCellStyle(IWorkbook workbook)
        {
            var cellStyle = workbook.CreateCellStyle();
            cellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd-MM-yy HH:mm:ss");
            return cellStyle;
        }

        private static void CreateDateCell(IRow dataRow, int rowIndex, DateTime? value, ICellStyle cellStyle)
        {
            var cell = dataRow.CreateCell(rowIndex);
            if (value is null)
            {
                cell.SetCellValue(string.Empty);
                return;
            }

            cell.SetCellValue((DateTime)value);
            cell.CellStyle = cellStyle;
        }
    }
}

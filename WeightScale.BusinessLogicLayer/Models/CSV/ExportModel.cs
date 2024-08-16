using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Models.CSV
{
    public class ExportModel
    {
        public Shipment Shipment { get; set; }
        public double PackageFullTotals { get; set; }
        public double PackageEmptyTotals { get; set; }
        public double PackageNetTotals { get; set; }
    }
}

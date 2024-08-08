using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Models.CSV
{
    public class ExportModel
    {
        public Shipment Shipment { get; set; }
        public string PackageFullTotals { get; set; }
        public string PackageEmptyTotals { get; set; }
        public string PackageNetTotals { get; set; }
    }
}

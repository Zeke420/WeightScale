using System;
using System.Collections.Generic;

namespace WeightScale.BusinessLogicLayer.Models.CSV
{
    public class ExportModel
    {
        public string ShipmentInfo { get; set; }
        public string PackageHeader { get; set; }
        public List<string> Packages { get; set; }
        public string PackageTotals { get; set; }
        public DateTime ShipmentDate { get; set; }
    }
}

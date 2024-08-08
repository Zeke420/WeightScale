using System.Collections.Generic;

namespace WeightScale.BusinessLogicLayer.Models
{
    public class PackageModel
    {
        public int Id { get; set; }
        public string FullWeight { get; set; }
        public string EmptyWeight { get; set; }
        public int ShipmentId { get; set; }
        public string WeightDifference { get; set; }

        public bool CanManualMeasure =>
            !string.IsNullOrEmpty(EmptyWeight);
        public List<PackageMoveModel> PackageMoves { get; set; }
    }
}

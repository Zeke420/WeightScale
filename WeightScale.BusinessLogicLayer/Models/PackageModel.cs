namespace WeightScale.BusinessLogicLayer.Models
{
    public class PackageModel
    {
        public int Id { get; set; }
        public double? FullWeight { get; set; }
        public double? EmptyWeight { get; set; }
        public int ShipmentId { get; set; }
        public double? WeightDifference => FullWeight - EmptyWeight;
    }
}

namespace WeightScale.DataAccessLayer.DTOs
{
    public class PackageWeights
    {
        public double FullWeight { get; set; }
        public double EmptyWeight { get; set; }
        public bool IsFilledOut => FullWeight > 0 && EmptyWeight > 0;
    }
}
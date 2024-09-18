namespace WeightScale.BusinessLogicLayer.Models
{
    public class ApplicationSettings
    {
        public string ConnectionString { get; set; }
        public string IpAddressFullWeight { get; set; }
        public string IpAddressEmptyWeight { get; set; }
        public string ReportFilePath { get; set; }
        public string LogFilePath { get; set; }
    }
}

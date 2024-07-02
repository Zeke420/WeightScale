using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Models
{
    public class CouriersSelectionModel
    {
        public Courier Courier { get; set; }
        public bool IsSelected { get; set; }
    }
}
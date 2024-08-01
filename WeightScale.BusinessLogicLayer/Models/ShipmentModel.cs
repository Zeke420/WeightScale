using System;
using System.Collections.Generic;
using WeightScale.DataAccessLayer.Entities;

namespace WeightScale.BusinessLogicLayer.Models
{
    public class ShipmentModel
    {
        public int Id { get; set; }
        public DateTime ShipmentDate { get; set; }
        public int CourierId { get; set; }
        public Courier Courier { get; set; }
        public bool IsFinished { get; set; }
        public List<PackageModel> Packages { get; set; }
    }
}

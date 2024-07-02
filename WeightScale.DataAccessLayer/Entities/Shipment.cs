using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeightScale.DataAccessLayer.Entities
{
    public class Shipment
    {
        public Shipment()
        {
        }
        
        public Shipment(DateTime shipmentDate, int courierId)
        {
            ShipmentDate = shipmentDate;
            CourierId = courierId;
        }
        
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DateTime ShipmentDate { get; set; }
        
        [ForeignKey(nameof(Courier))]
        public int CourierId { get; set; }
        
        public Courier Courier { get; set; }
        
        [InverseProperty(nameof(Package.Shipment))]
        public List<Package> Packages { get; set; }
        
        public double TotalWeight { get; set; }
        
        public bool IsFinished { get; set; }
    }
}
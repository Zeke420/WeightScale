using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeightScale.DataAccessLayer.Entities
{
    public class Package
    {
        [Key] public int Id { get; set; }

        public double? FullWeight { get; set; }
        public DateTime? FullPackageDate { get; set; }

        public double? EmptyWeight { get; set; }
        public DateTime? EmptyPackageDate { get; set; }

        [ForeignKey(nameof(Shipment))] public int ShipmentId { get; set; }

        public Shipment Shipment { get; set; }
    }
}

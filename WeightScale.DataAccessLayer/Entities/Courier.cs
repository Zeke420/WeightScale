using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeightScale.DataAccessLayer.Entities
{
    public class Courier
    {
        public Courier()
        {
        }

        public Courier(string name)
        {
            Name = name;
        }

        [Key] public int Id { get; set; }

        [Required] [StringLength(100)] public string Name { get; set; }

        [InverseProperty(nameof(Shipment.Courier))]
        public List<Shipment> Shipments { get; set; }
    }
}
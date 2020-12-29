namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Temperatures")]
    public class TemperatureEntity
    {
        [Column("TemperatureId"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int TemperatureId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}
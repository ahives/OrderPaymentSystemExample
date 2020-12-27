namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StorageTemperatures")]
    public class StorageTemperatureEntity
    {
        [Column("StorageTemperatureId"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int StorageTemperatureId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}
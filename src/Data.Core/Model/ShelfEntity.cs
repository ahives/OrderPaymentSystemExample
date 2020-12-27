namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Shelves")]
    public class ShelfEntity
    {
        [Column("ShelfId"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int ShelfId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        [ForeignKey("StorageTemperatureId"), Required]
        public int StorageTemperatureId { get; init; }
        public StorageTemperatureEntity StorageTemperature { get; init; }
        
        [Column("Capacity"), Required]
        public int Capacity { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}
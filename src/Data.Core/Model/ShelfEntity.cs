namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Shelves")]
    public class ShelfEntity
    {
        [Column("ShelfId"), Key, Required]
        public Guid ShelfId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        [ForeignKey("RestaurantId"), Required]
        public Guid RestaurantId { get; init; }
        public RestaurantEntity Restaurant { get; init; }
        
        [Column("IsOverflow")]
        public bool IsOverflow { get; init; }
        
        [Column("DecayRate"), Required]
        public decimal DecayRate { get; init; }
        
        [ForeignKey("TemperatureId"), Required]
        public Guid TemperatureId { get; init; }
        public TemperatureEntity Temperature { get; init; }
        
        [Column("Capacity"), Required]
        public int Capacity { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}
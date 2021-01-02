namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Ingredients")]
    public class IngredientEntity
    {
        [Column("IngredientId"), Key, Required]
        public Guid IngredientId { get; init; }
        
        [Column("Name"), Required]
        public string Name { get; init; }
        
        [ForeignKey("TemperatureId"), Required]
        public Guid TemperatureId { get; init; }
        public TemperatureEntity Temperature { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}
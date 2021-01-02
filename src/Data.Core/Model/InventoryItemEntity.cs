namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("InventoryItems")]
    public class InventoryItemEntity
    {
        [Column("InventoryItemId"), Key, Required]
        public Guid InventoryItemId { get; init; }
        
        [ForeignKey("RestaurantId"), Required]
        public Guid RestaurantId { get; init; }
        public RestaurantEntity Restaurant { get; init; }
        
        [Column("IngredientId"), Required]
        public Guid IngredientId { get; init; }
        public IngredientEntity Ingredient { get; init; }
        
        [Column("QuantityOnHand"), Required]
        public decimal QuantityOnHand { get; set; }
        
        [Column("ReplenishmentThreshold"), Required]
        public decimal ReplenishmentThreshold { get; set; }
        
        [Column("CreationTimestamp")]
        public DateTime CreationTimestamp { get; init; }
    }
}
namespace Data.Core.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MenuItemIngredients")]
    public class MenuItemIngredientEntity
    {
        [Column("MenuItemIngredientId"), Key, Required]
        public Guid MenuItemIngredientId { get; init; }
        
        [ForeignKey("MenuItemId"), Required]
        public Guid MenuItemId { get; init; }
        public MenuItemEntity MenuItem { get; init; }
        
        [ForeignKey("IngredientId"), Required]
        public Guid IngredientId { get; init; }
        public IngredientEntity Ingredient { get; init; }

        [Column("QuantityToUse")]
        public decimal QuantityToUse { get; init; }
        
        [Column("CreationTimestamp"), Required]
        public DateTime CreationTimestamp { get; init; }
    }
}
namespace Services.Core.Model
{
    using System;

    public record InventoryItem
    {
        public Guid InventoryItemId { get; init; }

        public Guid RestaurantId { get; init; }
        
        public Guid IngredientId { get; init; }
        
        public decimal QuantityOnHand { get; init; }
        
        public decimal ReplenishmentThreshold { get; init; }
    }
}
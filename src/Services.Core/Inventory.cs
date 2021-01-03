namespace Services.Core
{
    using System;
    using System.Collections.Generic;

    public record Inventory
    {
        public Guid RestaurantId { get; init; }
        
        public List<InventoryItem> Items { get; init; }
    }
}
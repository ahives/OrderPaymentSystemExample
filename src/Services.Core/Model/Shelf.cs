namespace Services.Core.Model
{
    using System;

    public record Shelf
    {
        public int ShelfId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public string Name { get; init; }
        
        public decimal DecayRate { get; init; }

        public bool IsOverflow { get; init; }
        
        public int TemperatureId { get; init; }
        
        public int Capacity { get; init; }
    }
}
namespace Services.Core.Model
{
    using System;

    public record Shelf
    {
        public Guid ShelfId { get; init; }
        
        public Guid RestaurantId { get; init; }
        
        public string Name { get; init; }
        
        public decimal DecayRate { get; init; }

        public bool IsOverflow { get; init; }
        
        public Guid TemperatureId { get; init; }
        
        public int Capacity { get; init; }
    }
}
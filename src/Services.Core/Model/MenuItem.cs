namespace Services.Core.Model
{
    using System;

    public record MenuItem
    {
        public Guid MenuItemId { get; init; }
        
        public string Name { get; init; }
        
        public decimal Price { get; init; }
        
        public Guid MenuId { get; init; }
        
        public decimal ShelfLife { get; set; }
        
        public Guid TemperatureId { get; init; }
    }
}
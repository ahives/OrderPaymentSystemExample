namespace Services.Core.Model
{
    public record Shelf
    {
        public int ShelfId { get; init; }
        
        public string Name { get; init; }
        
        public int StorageTemperatureId { get; init; }
        
        public int Capacity { get; init; }
    }
}
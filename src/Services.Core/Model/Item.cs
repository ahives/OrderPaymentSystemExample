namespace Services.Core.Model
{
    using System;

    public record Item
    {
        public Guid Id { get; init; }
        
        public int Status { get; init; }
        
        public string SpecialInstructions { get; init; }
    }
}
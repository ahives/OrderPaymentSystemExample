namespace Services.Core.Model
{
    using System;

    public record Courier
    {
        public Guid CourierId { get; init; }
        
        public string FirstName { get; init; }
        
        public string LastName { get; init; }
        
        public Address Address { get; init; }
    }
}
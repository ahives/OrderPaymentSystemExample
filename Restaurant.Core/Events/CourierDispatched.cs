namespace Restaurant.Core
{
    using System;

    public record CourierDispatched
    {
        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public int RestaurantId { get; init; }
        
        public int ShelfId { get; init; }
        
        public Guid[] Items { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
    public record OrderPickedUpByCourier
    {
        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public int RestaurantId { get; init; }
        
        public Guid[] Items { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
    public record CourierDeliveredOrder
    {
        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public int RestaurantId { get; init; }
        
        public Guid[] Items { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
    public record CourierConfirmed
    {
        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public int RestaurantId { get; init; }
        
        public Guid[] Items { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
    public record PreparedOrder
    {
        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public int RestaurantId { get; init; }
        
        public Guid[] Items { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
    public record OrderDiscarded
    {
        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public int RestaurantId { get; init; }
        
        public Guid[] Items { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
}
﻿namespace Restaurant.Core
{
    using System;

    public record SubmitOrder
    {
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public DateTime Timestamp { get; init; }
    }
}
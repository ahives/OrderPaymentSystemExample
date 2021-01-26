﻿namespace Services.Core.Events
{
    using System;

    public record SubmitOrder
    {
        public SubmitOrder()
        {
            Timestamp = DateTime.Now;
        }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid MenuItemId { get; init; }
        
        public DateTime Timestamp { get; }
    }
}
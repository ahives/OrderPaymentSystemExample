namespace OrderProcessingService.Core.StateMachines.Sagas
{
    using System;
    using System.Collections.Generic;
    using Automatonymous;

    public class OrderState :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        
        public int CurrentState { get; set; }
        
        public Guid CustomerId { get; set; }
        
        public Guid RestaurantId { get; set; }
        
        public Guid? CourierId { get; set; }
        
        public int ExpectedItemCount { get; set; }
        
        public int ActualItemCount { get; set; }
        
        public List<ExpectedOrderItem> Items { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public byte[] RowVersion { get; set; }
    }
}
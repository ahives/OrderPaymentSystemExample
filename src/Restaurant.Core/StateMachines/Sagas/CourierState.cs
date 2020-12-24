namespace Restaurant.Core.StateMachines.Sagas
{
    using System;
    using Automatonymous;

    public class CourierState :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        
        public int CurrentState { get; set; }
        
        public Guid CustomerId { get; set; }
        
        public Guid RestaurantId { get; set; }
        
        public Guid OrderId { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}
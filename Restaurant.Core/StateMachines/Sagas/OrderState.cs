namespace Restaurant.Core.StateMachines.Sagas
{
    using System;
    using Automatonymous;

    public class OrderState :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        
        public int CurrentState { get; set; }
        
        public Guid CustomerId { get; set; }
        
        public Guid? CourierId { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}
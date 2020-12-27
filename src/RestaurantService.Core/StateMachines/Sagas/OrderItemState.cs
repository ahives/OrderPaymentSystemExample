namespace RestaurantService.Core.StateMachines.Sagas
{
    using System;
    using Automatonymous;

    public class OrderItemState :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        
        public Guid OrderId { get; set; }
        
        public int CurrentState { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public byte[] RowVersion { get; set; }
    }
}
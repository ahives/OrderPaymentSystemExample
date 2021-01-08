namespace CourierService.Core.StateMachines.Sagas
{
    using System;
    using Automatonymous;

    public class CourierState :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        
        public int CurrentState { get; set; }
        
        public Guid? CourierId { get; set; }
        
        public Guid CustomerId { get; set; }
        
        public Guid RestaurantId { get; set; }
        
        public Guid OrderId { get; set; }
        
        public bool HasCourierArrived { get; set; }
        
        public bool IsOrderReady { get; set; }
        
        public Guid? OrderCompletionTimeoutTokenId { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public byte[] RowVersion { get; set; }
    }
}
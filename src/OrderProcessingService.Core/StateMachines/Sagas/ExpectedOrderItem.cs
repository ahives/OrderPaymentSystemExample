namespace OrderProcessingService.Core.StateMachines.Sagas
{
    using System;
    using Automatonymous;

    public class ExpectedOrderItem :
        SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        
        public Guid OrderId { get; set; }
        
        public int Status { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public byte[] RowVersion { get; set; }
    }
}
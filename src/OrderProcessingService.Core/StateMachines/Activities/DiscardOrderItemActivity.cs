namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class DiscardOrderItemActivity :
        Activity<OrderItemState, OrderItemDiscarded>
    {
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemDiscarded> context, Behavior<OrderItemState, OrderItemDiscarded> next) => throw new NotImplementedException();

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderItemState, OrderItemDiscarded, TException> context, Behavior<OrderItemState, OrderItemDiscarded> next) where TException : Exception => throw new NotImplementedException();
    }
}
namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class CancelOrderItemActivity :
        Activity<OrderItemState, OrderCanceled>
    {
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<OrderItemState, OrderCanceled> context, Behavior<OrderItemState, OrderCanceled> next) => throw new NotImplementedException();

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderItemState, OrderCanceled, TException> context, Behavior<OrderItemState, OrderCanceled> next) where TException : Exception => throw new NotImplementedException();
    }
}
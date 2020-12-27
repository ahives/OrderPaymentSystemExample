namespace RestaurantService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class PrepareOrderItemActivity :
        Activity<OrderItemState, PrepareOrderItem>
    {
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<OrderItemState, PrepareOrderItem> context, Behavior<OrderItemState, PrepareOrderItem> next) => throw new NotImplementedException();

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderItemState, PrepareOrderItem, TException> context, Behavior<OrderItemState, PrepareOrderItem> next) where TException : Exception => throw new NotImplementedException();
    }
}
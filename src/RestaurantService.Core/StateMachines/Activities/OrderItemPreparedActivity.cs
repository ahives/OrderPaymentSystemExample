namespace RestaurantService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemPreparedActivity :
        Activity<OrderItemState, OrderItemPrepared>
    {
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemPrepared> context, Behavior<OrderItemState, OrderItemPrepared> next) => throw new NotImplementedException();

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderItemState, OrderItemPrepared, TException> context, Behavior<OrderItemState, OrderItemPrepared> next) where TException : Exception => throw new NotImplementedException();
    }
}
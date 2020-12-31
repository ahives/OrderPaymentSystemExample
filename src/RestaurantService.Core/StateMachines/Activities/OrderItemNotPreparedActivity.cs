namespace RestaurantService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemNotPreparedActivity :
        Activity<OrderItemState, OrderItemNotPrepared>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemNotPrepared> context,
            Behavior<OrderItemState, OrderItemNotPrepared> next)
        {
            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, OrderItemNotPrepared, TException> context,
            Behavior<OrderItemState, OrderItemNotPrepared> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
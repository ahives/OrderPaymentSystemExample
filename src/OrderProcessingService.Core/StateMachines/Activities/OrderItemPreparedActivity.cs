namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class OrderItemPreparedActivity :
        Activity<OrderItemState, OrderItemPrepared>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemPrepared> context,
            Behavior<OrderItemState, OrderItemPrepared> next)
        {
            Log.Information($"Order Item State Machine - {nameof(OrderItemPreparedActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, OrderItemPrepared, TException> context,
            Behavior<OrderItemState, OrderItemPrepared> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
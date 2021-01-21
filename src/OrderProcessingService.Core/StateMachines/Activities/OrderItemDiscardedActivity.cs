namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class OrderItemDiscardedActivity :
        Activity<OrderItemState, OrderItemDiscarded>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemDiscarded> context,
            Behavior<OrderItemState, OrderItemDiscarded> next)
        {
            Log.Information($"Order Item State Machine - {nameof(OrderItemDiscardedActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, OrderItemDiscarded, TException> context,
            Behavior<OrderItemState, OrderItemDiscarded> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
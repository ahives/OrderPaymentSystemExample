namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class CancelOrderItemActivity :
        Activity<OrderItemState, OrderCanceled>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderItemState, OrderCanceled> context,
            Behavior<OrderItemState, OrderCanceled> next)
        {
            Log.Information($"Order Item State Machine - {nameof(CancelOrderItemActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, OrderCanceled, TException> context,
            Behavior<OrderItemState, OrderCanceled> next)
            where TException : Exception
        {
            
        }
    }
}
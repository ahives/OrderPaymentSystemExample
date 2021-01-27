namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class OrderItemsExpiredActivity :
        Activity<OrderState, OrderItemExpired>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderItemExpired> context,
            Behavior<OrderState, OrderItemExpired> next)
        {
            Log.Information($"Order State Machine - {nameof(OrderItemsExpiredActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderState, OrderItemExpired, TException> context,
            Behavior<OrderState, OrderItemExpired> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
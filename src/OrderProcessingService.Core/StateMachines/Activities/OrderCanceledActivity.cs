namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class OrderCanceledActivity :
        Activity<OrderState, OrderCanceled>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderCanceled> context,
            Behavior<OrderState, OrderCanceled> next)
        {
            Log.Information($"Order State Machine - {nameof(OrderCanceledActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderCanceled, TException> context,
            Behavior<OrderState, OrderCanceled> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
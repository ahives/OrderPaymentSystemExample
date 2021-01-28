namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderCanceledActivity :
        Activity<OrderState, OrderCanceled>
    {
        readonly ILogger<OrderCanceledActivity> _logger;

        public OrderCanceledActivity(ILogger<OrderCanceledActivity> logger)
        {
            _logger = logger;
        }

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
            _logger.LogInformation($"Order State Machine - {nameof(OrderCanceledActivity)} (state = {context.Instance.CurrentState})");

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
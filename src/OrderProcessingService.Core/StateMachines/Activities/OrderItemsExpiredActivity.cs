namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemsExpiredActivity :
        Activity<OrderState, OrderItemExpired>
    {
        readonly ILogger<OrderItemsExpiredActivity> _logger;

        public OrderItemsExpiredActivity(ILogger<OrderItemsExpiredActivity> logger)
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

        public async Task Execute(BehaviorContext<OrderState, OrderItemExpired> context,
            Behavior<OrderState, OrderItemExpired> next)
        {
            _logger.LogInformation($"Order State Machine - {nameof(OrderItemsExpiredActivity)} (state = {context.Instance.CurrentState})");
            
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
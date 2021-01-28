namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemPreparedActivity :
        Activity<OrderItemState, OrderItemPrepared>
    {
        readonly ILogger<OrderItemPreparedActivity> _logger;

        public OrderItemPreparedActivity(ILogger<OrderItemPreparedActivity> logger)
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

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemPrepared> context,
            Behavior<OrderItemState, OrderItemPrepared> next)
        {
            _logger.LogInformation($"Order Item State Machine - {nameof(OrderItemPreparedActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;
            
            await next.Execute(context).ConfigureAwait(false);
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
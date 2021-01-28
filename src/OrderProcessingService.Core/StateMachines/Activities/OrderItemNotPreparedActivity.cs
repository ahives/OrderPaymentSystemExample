namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemNotPreparedActivity :
        Activity<OrderItemState, OrderItemNotPrepared>
    {
        readonly ILogger<OrderItemNotPreparedActivity> _logger;

        public OrderItemNotPreparedActivity(ILogger<OrderItemNotPreparedActivity> logger)
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

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemNotPrepared> context,
            Behavior<OrderItemState, OrderItemNotPrepared> next)
        {
            _logger.LogInformation($"Order Item State Machine - {nameof(OrderItemNotPreparedActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, OrderItemNotPrepared, TException> context,
            Behavior<OrderItemState, OrderItemNotPrepared> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
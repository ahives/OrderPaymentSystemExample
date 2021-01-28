namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemDiscardedActivity :
        Activity<OrderItemState, OrderItemDiscarded>
    {
        readonly ILogger<OrderItemDiscardedActivity> _logger;

        public OrderItemDiscardedActivity(ILogger<OrderItemDiscardedActivity> logger)
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

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemDiscarded> context,
            Behavior<OrderItemState, OrderItemDiscarded> next)
        {
            _logger.LogInformation($"Order Item State Machine - {nameof(OrderItemDiscardedActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
            
            await next.Execute(context).ConfigureAwait(false);
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
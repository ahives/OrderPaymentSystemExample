namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemVoidedActivity :
        Activity<OrderItemState, OrderItemVoided>
    {
        readonly ILogger<OrderItemVoidedActivity> _logger;

        public OrderItemVoidedActivity(ILogger<OrderItemVoidedActivity> logger)
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

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemVoided> context,
            Behavior<OrderItemState, OrderItemVoided> next)
        {
            _logger.LogInformation($"Order Item State Machine - {nameof(OrderItemVoidedActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, OrderItemVoided, TException> context,
            Behavior<OrderItemState, OrderItemVoided> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
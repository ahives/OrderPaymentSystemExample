namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemCanceledActivity :
        Activity<OrderItemState, OrderItemCanceled>
    {
        readonly ILogger<OrderItemCanceledActivity> _logger;

        public OrderItemCanceledActivity(ILogger<OrderItemCanceledActivity> logger)
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

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemCanceled> context,
            Behavior<OrderItemState, OrderItemCanceled> next)
        {
            _logger.LogInformation($"Order Item State Machine - {nameof(OrderItemCanceledActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, OrderItemCanceled, TException> context,
            Behavior<OrderItemState, OrderItemCanceled> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
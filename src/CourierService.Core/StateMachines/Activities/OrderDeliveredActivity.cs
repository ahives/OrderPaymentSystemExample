namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderDeliveredActivity :
        Activity<CourierState, OrderDelivered>
    {
        readonly ILogger<OrderDeliveredActivity> _logger;

        public OrderDeliveredActivity(ILogger<OrderDeliveredActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, OrderDelivered> context,
            Behavior<CourierState, OrderDelivered> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(OrderDeliveredActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, OrderDelivered, TException> context,
            Behavior<CourierState, OrderDelivered> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
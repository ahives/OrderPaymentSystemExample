namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderExpiredActivity :
        Activity<CourierState, OrderExpired>
    {
        readonly ILogger<OrderExpiredActivity> _logger;

        public OrderExpiredActivity(ILogger<OrderExpiredActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, OrderExpired> context,
            Behavior<CourierState, OrderExpired> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(OrderExpiredActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.CourierId = null;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CourierState, OrderExpired, TException> context,
            Behavior<CourierState, OrderExpired> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
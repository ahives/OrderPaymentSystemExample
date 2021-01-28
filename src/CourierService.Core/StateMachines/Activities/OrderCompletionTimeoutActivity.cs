namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderCompletionTimeoutActivity :
        Activity<CourierState, OrderCompletionTimeoutExpired>
    {
        readonly ConsumeContext _context;
        readonly ILogger<OrderCompletionTimeoutActivity> _logger;

        public OrderCompletionTimeoutActivity(ConsumeContext context, ILogger<OrderCompletionTimeoutActivity> logger)
        {
            _context = context;
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

        public async Task Execute(BehaviorContext<CourierState, OrderCompletionTimeoutExpired> context,
            Behavior<CourierState, OrderCompletionTimeoutExpired> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(OrderCompletionTimeoutActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.IsOrderReady = false;

            await _context.Publish<DeclineCourierDispatch>(new
            {
                context.Data.CourierId,
                context.Data.OrderId,
                context.Data.CustomerId,
                context.Data.RestaurantId
            });
            
            _logger.LogInformation($"Published - {nameof(DeclineCourierDispatch)}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, OrderCompletionTimeoutExpired, TException> context,
            Behavior<CourierState, OrderCompletionTimeoutExpired> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
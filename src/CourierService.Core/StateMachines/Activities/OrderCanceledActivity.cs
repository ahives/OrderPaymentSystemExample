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

    public class OrderCanceledActivity :
        Activity<CourierState, OrderCanceled>
    {
        readonly ConsumeContext _context;
        readonly ILogger<OrderCanceledActivity> _logger;

        public OrderCanceledActivity(ConsumeContext context, ILogger<OrderCanceledActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, OrderCanceled> context,
            Behavior<CourierState, OrderCanceled> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(OrderCanceledActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.CourierId = null;
            context.Instance.IsOrderReady = false;

            await _context.Publish<CancelCourierDispatch>(
                new
                {
                    context.Instance.CourierId,
                    context.Instance.OrderId,
                    context.Instance.CustomerId,
                    context.Instance.RestaurantId
                });
            
            _logger.LogInformation($"Published - {nameof(CancelCourierDispatch)}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CourierState, OrderCanceled, TException> context,
            Behavior<CourierState, OrderCanceled> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
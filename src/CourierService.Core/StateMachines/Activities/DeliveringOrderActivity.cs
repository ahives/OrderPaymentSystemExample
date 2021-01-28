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

    public class DeliveringOrderActivity :
        Activity<CourierState, DeliveringOrder>
    {
        readonly ConsumeContext _context;
        readonly ILogger<DeliveringOrderActivity> _logger;

        public DeliveringOrderActivity(ConsumeContext context, ILogger<DeliveringOrderActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, DeliveringOrder> context,
            Behavior<CourierState, DeliveringOrder> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(DeliveringOrderActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;

            await _context.Publish<DeliverOrder>(
                new
                {
                    context.Data.CourierId,
                    context.Data.OrderId,
                    context.Data.CustomerId,
                    context.Data.RestaurantId
                });
            
            _logger.LogInformation($"Published - {nameof(DeliverOrder)}");
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, DeliveringOrder, TException> context,
            Behavior<CourierState, DeliveringOrder> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
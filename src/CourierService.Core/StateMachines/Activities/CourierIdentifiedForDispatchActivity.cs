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

    public class CourierIdentifiedForDispatchActivity :
        Activity<CourierState, CourierIdentifiedForDispatch>
    {
        readonly ConsumeContext _context;
        readonly ILogger<CourierIdentifiedForDispatchActivity> _logger;

        public CourierIdentifiedForDispatchActivity(ConsumeContext context, ILogger<CourierIdentifiedForDispatchActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, CourierIdentifiedForDispatch> context,
            Behavior<CourierState, CourierIdentifiedForDispatch> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(CourierIdentifiedForDispatchActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
            
            if (context.Instance.DispatchAttempts < context.Instance.MaxDispatchAttempts)
            {
                await _context.Publish<DispatchCourier>(
                    new
                    {
                        context.Data.CourierId,
                        context.Data.OrderId,
                        context.Data.CustomerId,
                        context.Data.RestaurantId
                    });

                _logger.LogInformation($"Published - {nameof(DispatchCourier)}");
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierIdentifiedForDispatch, TException> context,
            Behavior<CourierState, CourierIdentifiedForDispatch> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
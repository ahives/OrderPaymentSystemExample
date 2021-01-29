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

    public class IdentifyCourierForDispatchActivity :
        Activity<CourierState, IdentifyCourierForDispatch>
    {
        readonly ConsumeContext _context;
        readonly ILogger<IdentifyCourierForDispatchActivity> _logger;

        public IdentifyCourierForDispatchActivity(ConsumeContext context, ILogger<IdentifyCourierForDispatchActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, IdentifyCourierForDispatch> context,
            Behavior<CourierState, IdentifyCourierForDispatch> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(IdentifyCourierForDispatchActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;

            await _context.Publish<IdentifyCourierForDispatch>(
                new
                {
                    context.Data.OrderId,
                    context.Data.CustomerId,
                    context.Data.RestaurantId
                });

            _logger.LogInformation($"Published - {nameof(IdentifyCourierForDispatch)}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, IdentifyCourierForDispatch, TException> context,
            Behavior<CourierState, IdentifyCourierForDispatch> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
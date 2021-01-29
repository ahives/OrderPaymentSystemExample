namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class EnRouteToRestaurantActivity :
        Activity<CourierState, CourierEnRouteToRestaurant>
    {
        readonly ILogger<EnRouteToRestaurantActivity> _logger;

        public EnRouteToRestaurantActivity(ILogger<EnRouteToRestaurantActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, CourierEnRouteToRestaurant> context,
            Behavior<CourierState, CourierEnRouteToRestaurant> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(EnRouteToRestaurantActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierEnRouteToRestaurant, TException> context,
            Behavior<CourierState, CourierEnRouteToRestaurant> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
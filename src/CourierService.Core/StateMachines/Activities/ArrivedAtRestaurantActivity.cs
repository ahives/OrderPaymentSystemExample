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

    public class ArrivedAtRestaurantActivity :
        Activity<CourierState, CourierArrivedAtRestaurant>
    {
        readonly ConsumeContext _context;
        readonly ILogger<ArrivedAtRestaurantActivity> _logger;

        public ArrivedAtRestaurantActivity(ConsumeContext context, ILogger<ArrivedAtRestaurantActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, CourierArrivedAtRestaurant> context,
            Behavior<CourierState, CourierArrivedAtRestaurant> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(ArrivedAtRestaurantActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;
            context.Instance.HasCourierArrived = true;

            if (context.Instance.IsOrderReady)
            {
                await _context.Publish<PickUpOrder>(
                    new()
                    {
                        CourierId = context.Data.CourierId,
                        RestaurantId = context.Data.RestaurantId,
                        CustomerId = context.Data.CustomerId,
                        OrderId = context.Data.OrderId
                    });
                
                _logger.LogInformation($"Published - {nameof(PickUpOrder)}");
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierArrivedAtRestaurant, TException> context,
            Behavior<CourierState, CourierArrivedAtRestaurant> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
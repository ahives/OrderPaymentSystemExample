namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class CourierArrivedAtRestaurantActivity :
        Activity<CourierState, CourierArrivedAtRestaurant>
    {
        readonly ConsumeContext _context;

        public CourierArrivedAtRestaurantActivity(ConsumeContext context)
        {
            _context = context;
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
            Log.Information($"Courier State Machine - {nameof(OrderExpiredActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.HasCourierArrived = true;

            await _context.Send<UpdateCourierStatus>(new()
            {
                CourierId = context.Data.CourierId,
                RestaurantId = context.Data.RestaurantId,
                CustomerId = context.Data.CustomerId,
                OrderId = context.Data.OrderId,
                Status = (int)CourierStatus.EnRouteToRestaurant
            });

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
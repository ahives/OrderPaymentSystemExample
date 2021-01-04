namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class CourierEnRouteToRestaurantActivity :
        Activity<CourierState, CourierEnRouteToRestaurant>
    {
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
            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierEnRouteToRestaurant, TException> context,
            Behavior<CourierState, CourierEnRouteToRestaurant> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
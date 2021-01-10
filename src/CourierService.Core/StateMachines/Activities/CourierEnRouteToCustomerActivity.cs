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

    public class CourierEnRouteToCustomerActivity :
        Activity<CourierState, CourierEnRouteToCustomer>
    {
        readonly ConsumeContext _context;

        public CourierEnRouteToCustomerActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, CourierEnRouteToCustomer> context,
            Behavior<CourierState, CourierEnRouteToCustomer> next)
        {
            Log.Information($"Courier State Machine - {nameof(CourierEnRouteToCustomerActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;

            await _context.Publish<UpdateCourierStatus>(new()
            {
                CourierId = context.Data.CourierId,
                RestaurantId = context.Data.RestaurantId,
                CustomerId = context.Data.CustomerId,
                OrderId = context.Data.OrderId,
                Status = CourierStatus.EnRouteToCustomer
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierEnRouteToCustomer, TException> context,
            Behavior<CourierState, CourierEnRouteToCustomer> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
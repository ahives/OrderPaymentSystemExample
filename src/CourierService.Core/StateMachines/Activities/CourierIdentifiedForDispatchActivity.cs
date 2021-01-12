namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class CourierIdentifiedForDispatchActivity :
        Activity<CourierState, CourierIdentifiedForDispatch>
    {
        readonly ConsumeContext _context;

        public CourierIdentifiedForDispatchActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, CourierIdentifiedForDispatch> context,
            Behavior<CourierState, CourierIdentifiedForDispatch> next)
        {
            Log.Information($"Courier State Machine - {nameof(CourierIdentifiedForDispatchActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            
            await _context.Publish<DispatchCourier>(new
            {
                context.Data.CourierId,
                context.Data.OrderId,
                context.Data.CustomerId,
                context.Data.RestaurantId
            });

            Log.Information($"Sent - {nameof(DispatchCourier)}");
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
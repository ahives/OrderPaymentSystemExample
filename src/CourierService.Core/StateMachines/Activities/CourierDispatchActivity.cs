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

    public class CourierDispatchActivity :
        Activity<CourierState, CourierDispatched>
    {
        readonly ConsumeContext _context;

        public CourierDispatchActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, CourierDispatched> context,
            Behavior<CourierState, CourierDispatched> next)
        {
            Log.Information($"Courier State Machine - {nameof(CourierDispatchActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.RestaurantId = context.Data.RestaurantId;
            context.Instance.CustomerId = context.Data.CustomerId;
            context.Instance.OrderId = context.Data.OrderId;
            context.Instance.CourierId = context.Data.CourierId;
            
            await _context.Send<ConfirmCourierDispatch>(new
            {
                context.Data.CourierId,
                context.Data.OrderId,
                context.Data.CustomerId,
                context.Data.RestaurantId
            });

            Log.Information($"Sent {nameof(ConfirmCourierDispatch)}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierDispatched, TException> context,
            Behavior<CourierState, CourierDispatched> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
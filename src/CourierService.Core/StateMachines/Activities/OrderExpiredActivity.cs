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

    public class OrderExpiredActivity :
        Activity<CourierState, OrderExpired>
    {
        readonly ConsumeContext _context;

        public OrderExpiredActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, OrderExpired> context,
            Behavior<CourierState, OrderExpired> next)
        {
            Log.Information($"Courier State Machine - {nameof(OrderExpiredActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;

            // await _context.Publish<CourierCanceled>(new
            // {
            //     CourierId = context.Instance.OrderId,
            //     context.Instance.OrderId,
            //     context.Instance.CustomerId,
            //     context.Instance.RestaurantId,
            //     Timestamp = DateTime.Now
            // });

            Log.Information($"Published {nameof(OrderExpired)}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CourierState, OrderExpired, TException> context,
            Behavior<CourierState, OrderExpired> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
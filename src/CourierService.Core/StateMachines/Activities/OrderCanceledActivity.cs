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

    public class OrderCanceledActivity :
        Activity<CourierState, OrderCanceled>
    {
        readonly ConsumeContext _context;

        public OrderCanceledActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, OrderCanceled> context,
            Behavior<CourierState, OrderCanceled> next)
        {
            Log.Information($"Courier State Machine - {nameof(OrderCanceledActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.CourierId = null;
            context.Instance.IsOrderReady = false;

            await _context.Publish<CourierDispatchCanceled>(new
            {
                context.Instance.CourierId,
                context.Instance.OrderId,
                context.Instance.CustomerId,
                context.Instance.RestaurantId
            });

            Log.Information($"Published - {nameof(CourierDispatchCanceled)}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CourierState, OrderCanceled, TException> context,
            Behavior<CourierState, OrderCanceled> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
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

    public class OrderCompletionTimeoutActivity :
        Activity<CourierState, OrderCompletionTimeoutExpired>
    {
        readonly ConsumeContext _context;

        public OrderCompletionTimeoutActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, OrderCompletionTimeoutExpired> context,
            Behavior<CourierState, OrderCompletionTimeoutExpired> next)
        {
            Log.Information($"Courier State Machine - {nameof(CourierDispatchActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.IsOrderReady = false;

            await _context.Send<CourierDispatchDeclined>(new
            {
                context.Data.CourierId,
                context.Data.OrderId,
                context.Data.CustomerId,
                context.Data.RestaurantId
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, OrderCompletionTimeoutExpired, TException> context,
            Behavior<CourierState, OrderCompletionTimeoutExpired> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
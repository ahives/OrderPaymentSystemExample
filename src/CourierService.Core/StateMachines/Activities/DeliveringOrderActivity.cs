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

    public class DeliveringOrderActivity :
        Activity<CourierState, DeliveringOrder>
    {
        readonly ConsumeContext _context;

        public DeliveringOrderActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, DeliveringOrder> context,
            Behavior<CourierState, DeliveringOrder> next)
        {
            Log.Information($"Courier State Machine - {nameof(DeliveringOrderActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            
            await _context.Publish<DeliverOrder>(new
            {
                context.Data.CourierId,
                context.Data.OrderId,
                context.Data.CustomerId,
                context.Data.RestaurantId
            });
            
            Log.Information($"Published - {nameof(DeliverOrder)}");
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, DeliveringOrder, TException> context,
            Behavior<CourierState, DeliveringOrder> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
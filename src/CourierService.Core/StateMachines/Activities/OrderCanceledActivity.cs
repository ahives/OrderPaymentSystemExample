namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
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
            context.Instance.Timestamp = DateTime.Now;

            await _context.Publish<CourierCanceled>(new
            {
                context.Instance.CourierId,
                context.Instance.OrderId,
                context.Instance.CustomerId,
                context.Instance.RestaurantId
            });
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CourierState, OrderCanceled, TException> context,
            Behavior<CourierState, OrderCanceled> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
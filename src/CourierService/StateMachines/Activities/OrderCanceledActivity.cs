namespace CourierService.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Restaurant.Core;
    using Sagas;

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
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<CourierState, OrderCanceled> context,
            Behavior<CourierState, OrderCanceled> next)
        {
            await _context.Publish<CourierRecalled>(new
            {
                CourierId = context.Instance.OrderId,
                context.Instance.OrderId,
                context.Instance.CustomerId,
                context.Instance.RestaurantId,
                Timestamp = DateTime.Now
            });
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CourierState, OrderCanceled, TException> context, Behavior<CourierState, OrderCanceled> next) where TException : Exception => throw new NotImplementedException();
    }
}
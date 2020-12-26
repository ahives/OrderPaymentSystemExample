namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
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
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<CourierState, OrderExpired> context,
            Behavior<CourierState, OrderExpired> next)
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

        public async Task Faulted<TException>(BehaviorExceptionContext<CourierState, OrderExpired, TException> context, Behavior<CourierState, OrderExpired> next) where TException : Exception => throw new NotImplementedException();
    }
}
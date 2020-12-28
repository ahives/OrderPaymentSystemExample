namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Services.Core.Events;

    public class OrderPickedUpByActivity :
        Activity<CourierState, OrderPickedUp>
    {
        readonly ConsumeContext _context;

        public OrderPickedUpByActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, OrderPickedUp> context,
            Behavior<CourierState, OrderPickedUp> next)
        {
            context.Instance.Timestamp = DateTime.Now;
            
            await _context.Send<DeliverOrder>(new
            {
                context.Data.OrderId,
                context.Data.CustomerId,
                context.Data.RestaurantId
            });
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CourierState, OrderPickedUp, TException> context, Behavior<CourierState, OrderPickedUp> next) where TException : Exception => throw new NotImplementedException();
    }
}
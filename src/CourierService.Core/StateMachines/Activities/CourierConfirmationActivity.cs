namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Services.Core.Events;

    public class CourierConfirmationActivity :
        Activity<CourierState, CourierConfirmed>
    {
        readonly ConsumeContext _context;

        public CourierConfirmationActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, CourierConfirmed> context,
            Behavior<CourierState, CourierConfirmed> next)
        {
            context.Instance.Timestamp = DateTime.Now;
            
            await _context.Send<PickUpOrder>(new
            {
                context.Data.OrderId,
                context.Data.CustomerId,
                context.Data.RestaurantId
            });
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierConfirmed, TException> context,
            Behavior<CourierState, CourierConfirmed> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
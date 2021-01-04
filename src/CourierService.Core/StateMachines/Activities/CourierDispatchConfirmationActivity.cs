namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Services.Core.Events;

    public class CourierDispatchConfirmationActivity :
        Activity<CourierState, CourierDispatchConfirmed>
    {
        readonly ConsumeContext _context;

        public CourierDispatchConfirmationActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, CourierDispatchConfirmed> context,
            Behavior<CourierState, CourierDispatchConfirmed> next)
        {
            context.Instance.Timestamp = DateTime.Now;
            
            // await _context.Send<PickUpOrder>(new
            // {
            //     context.Data.OrderId,
            //     context.Data.CustomerId,
            //     context.Data.RestaurantId
            // });
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierDispatchConfirmed, TException> context,
            Behavior<CourierState, CourierDispatchConfirmed> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class OrderDeliveryActivity :
        Activity<CourierState, OrderDelivered>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<CourierState, OrderDelivered> context,
            Behavior<CourierState, OrderDelivered> next)
        {
            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, OrderDelivered, TException> context,
            Behavior<CourierState, OrderDelivered> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
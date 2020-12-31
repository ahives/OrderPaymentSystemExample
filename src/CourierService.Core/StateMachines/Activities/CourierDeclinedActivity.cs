namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class CourierDeclinedActivity :
        Activity<CourierState, CourierDeclined>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<CourierState, CourierDeclined> context,
            Behavior<CourierState, CourierDeclined> next)
        {
            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierDeclined, TException> context,
            Behavior<CourierState, CourierDeclined> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
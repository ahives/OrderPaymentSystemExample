namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class CourierDeclinedActivity :
        Activity<CourierState, CourierDispatchDeclined>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<CourierState, CourierDispatchDeclined> context,
            Behavior<CourierState, CourierDispatchDeclined> next)
        {
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.CourierId = null;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierDispatchDeclined, TException> context,
            Behavior<CourierState, CourierDispatchDeclined> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
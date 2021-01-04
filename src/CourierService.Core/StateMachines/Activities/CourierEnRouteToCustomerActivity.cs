namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class CourierEnRouteToCustomerActivity :
        Activity<CourierState, CourierEnRouteToCustomer>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<CourierState, CourierEnRouteToCustomer> context,
            Behavior<CourierState, CourierEnRouteToCustomer> next)
        {
            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierEnRouteToCustomer, TException> context,
            Behavior<CourierState, CourierEnRouteToCustomer> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
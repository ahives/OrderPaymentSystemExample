namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class DispatchCanceledActivity :
        Activity<CourierState, CourierDispatchCanceled>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<CourierState, CourierDispatchCanceled> context,
            Behavior<CourierState, CourierDispatchCanceled> next)
        {
            Log.Information($"Courier State Machine - {nameof(DispatchCanceledActivity)}");

            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierDispatchCanceled, TException> context,
            Behavior<CourierState, CourierDispatchCanceled> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class CourierNotIdentifiedForDispatchActivity :
        Activity<CourierState, CourierNotIdentifiedForDispatch>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<CourierState, CourierNotIdentifiedForDispatch> context,
            Behavior<CourierState, CourierNotIdentifiedForDispatch> next)
        {
            Log.Information($"Courier State Machine - {nameof(CourierNotIdentifiedForDispatchActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierNotIdentifiedForDispatch, TException> context,
            Behavior<CourierState, CourierNotIdentifiedForDispatch> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
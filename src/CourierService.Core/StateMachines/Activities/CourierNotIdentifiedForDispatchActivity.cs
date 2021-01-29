namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class CourierNotIdentifiedForDispatchActivity :
        Activity<CourierState, CourierNotIdentifiedForDispatch>
    {
        readonly ILogger<CourierNotIdentifiedForDispatchActivity> _logger;

        public CourierNotIdentifiedForDispatchActivity(ILogger<CourierNotIdentifiedForDispatchActivity> logger)
        {
            _logger = logger;
        }

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
            _logger.LogInformation($"Courier State Machine - {nameof(CourierNotIdentifiedForDispatchActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;

            await next.Execute(context).ConfigureAwait(false);
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
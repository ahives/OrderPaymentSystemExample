namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class DispatchDeclinedActivity :
        Activity<CourierState, CourierDispatchDeclined>
    {
        readonly ILogger<DispatchDeclinedActivity> _logger;

        public DispatchDeclinedActivity(ILogger<DispatchDeclinedActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, CourierDispatchDeclined> context,
            Behavior<CourierState, CourierDispatchDeclined> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(DispatchDeclinedActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.CourierId = null;
            context.Instance.DispatchAttempts += 1;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierDispatchDeclined, TException> context,
            Behavior<CourierState, CourierDispatchDeclined> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
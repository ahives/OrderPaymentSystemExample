namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class DispatchConfirmationActivity :
        Activity<CourierState, CourierDispatchConfirmed>
    {
        readonly ILogger<DispatchConfirmationActivity> _logger;

        public DispatchConfirmationActivity(ILogger<DispatchConfirmationActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, CourierDispatchConfirmed> context,
            Behavior<CourierState, CourierDispatchConfirmed> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(DispatchConfirmationActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;

            await next.Execute(context).ConfigureAwait(false);
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
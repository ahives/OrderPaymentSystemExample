namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class DispatchCanceledActivity :
        Activity<CourierState, CourierDispatchCanceled>
    {
        readonly ILogger<DispatchCanceledActivity> _logger;

        public DispatchCanceledActivity(ILogger<DispatchCanceledActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, CourierDispatchCanceled> context,
            Behavior<CourierState, CourierDispatchCanceled> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(DispatchCanceledActivity)}");

            context.Instance.Timestamp = DateTime.Now;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierDispatchCanceled, TException> context,
            Behavior<CourierState, CourierDispatchCanceled> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
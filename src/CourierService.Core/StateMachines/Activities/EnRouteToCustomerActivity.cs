namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class EnRouteToCustomerActivity :
        Activity<CourierState, CourierEnRouteToCustomer>
    {
        readonly ILogger<EnRouteToCustomerActivity> _logger;

        public EnRouteToCustomerActivity(ILogger<EnRouteToCustomerActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, CourierEnRouteToCustomer> context,
            Behavior<CourierState, CourierEnRouteToCustomer> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(EnRouteToCustomerActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierEnRouteToCustomer, TException> context,
            Behavior<CourierState, CourierEnRouteToCustomer> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
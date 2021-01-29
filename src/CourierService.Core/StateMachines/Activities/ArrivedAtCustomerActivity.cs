namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class ArrivedAtCustomerActivity :
        Activity<CourierState, CourierArrivedAtCustomer>
    {
        readonly ILogger<ArrivedAtCustomerActivity> _logger;

        public ArrivedAtCustomerActivity(ILogger<ArrivedAtCustomerActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, CourierArrivedAtCustomer> context,
            Behavior<CourierState, CourierArrivedAtCustomer> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(ArrivedAtCustomerActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierArrivedAtCustomer, TException> context,
            Behavior<CourierState, CourierArrivedAtCustomer> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
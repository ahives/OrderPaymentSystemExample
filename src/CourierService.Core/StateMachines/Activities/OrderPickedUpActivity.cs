namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderPickedUpActivity :
        Activity<CourierState, OrderPickedUp>
    {
        readonly ILogger<OrderPickedUpActivity> _logger;

        public OrderPickedUpActivity(ILogger<OrderPickedUpActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, OrderPickedUp> context,
            Behavior<CourierState, OrderPickedUp> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(OrderPickedUpActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<CourierState, OrderPickedUp, TException> context,
            Behavior<CourierState, OrderPickedUp> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
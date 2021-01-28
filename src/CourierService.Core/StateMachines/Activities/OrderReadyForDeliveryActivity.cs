namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderReadyForDeliveryActivity :
        Activity<CourierState, OrderReadyForDelivery>
    {
        readonly ILogger<OrderReadyForDeliveryActivity> _logger;

        public OrderReadyForDeliveryActivity(ILogger<OrderReadyForDeliveryActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, OrderReadyForDelivery> context,
            Behavior<CourierState, OrderReadyForDelivery> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(OrderReadyForDeliveryActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.IsOrderReady = true;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, OrderReadyForDelivery, TException> context,
            Behavior<CourierState, OrderReadyForDelivery> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
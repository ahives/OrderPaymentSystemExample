namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class DispatchedActivity :
        Activity<CourierState, CourierDispatched>
    {
        readonly ILogger<DispatchedActivity> _logger;

        public DispatchedActivity(ILogger<DispatchedActivity> logger)
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

        public async Task Execute(BehaviorContext<CourierState, CourierDispatched> context,
            Behavior<CourierState, CourierDispatched> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(DispatchedActivity)} (state = {context.Instance.CurrentState})");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.RestaurantId = context.Data.RestaurantId;
            context.Instance.CustomerId = context.Data.CustomerId;
            context.Instance.OrderId = context.Data.OrderId;
            context.Instance.CourierId = context.Data.CourierId;
            context.Instance.IsOrderReady = false;
            context.Instance.HasCourierArrived = false;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, CourierDispatched, TException> context,
            Behavior<CourierState, CourierDispatched> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
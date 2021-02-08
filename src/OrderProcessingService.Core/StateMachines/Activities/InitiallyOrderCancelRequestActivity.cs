namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class InitiallyOrderCancelRequestActivity :
        Activity<OrderState, OrderCancelRequest>
    {
        readonly ILogger<InitiallyOrderCancelRequestActivity> _logger;

        public InitiallyOrderCancelRequestActivity(ILogger<InitiallyOrderCancelRequestActivity> logger)
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

        public async Task Execute(BehaviorContext<OrderState, OrderCancelRequest> context,
            Behavior<OrderState, OrderCancelRequest> next)
        {
            _logger.LogInformation($"Order State Machine - {nameof(InitiallyOrderCancelRequestActivity)} (state = {context.Instance.CurrentState})");
            _logger.LogInformation($"OrderId - {context.Data.OrderId}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.CustomerId = context.Data.CustomerId;
            context.Instance.RestaurantId = context.Data.RestaurantId;
            context.Instance.ExpectedItemCount = 0;
            context.Instance.PreparedItemCount = 0;
            context.Instance.CanceledItemCount = 0;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderState, OrderCancelRequest, TException> context,
            Behavior<OrderState, OrderCancelRequest> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemCancelRequestActivity :
        Activity<OrderItemState, OrderItemCancelRequest>
    {
        readonly ConsumeContext _context;
        readonly ILogger<OrderItemCancelRequestActivity> _logger;

        public OrderItemCancelRequestActivity(ConsumeContext context, ILogger<OrderItemCancelRequestActivity> logger)
        {
            _context = context;
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

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemCancelRequest> context,
            Behavior<OrderItemState, OrderItemCancelRequest> next)
        {
            _logger.LogInformation($"Order Item State Machine - {nameof(OrderItemCancelRequestActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;

            await _context.Publish<CancelOrderItem>(
                new()
                {
                    OrderId = context.Data.OrderId,
                    OrderItemId = context.Data.OrderItemId,
                    CustomerId = context.Data.CustomerId,
                    RestaurantId = context.Data.RestaurantId
                });
            
            _logger.LogInformation($"Published - {nameof(CancelOrderItem)}");
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, OrderItemCancelRequest, TException> context,
            Behavior<OrderItemState, OrderItemCancelRequest> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
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

    public class RequestOrderItemPreparationActivity :
        Activity<OrderItemState, RequestOrderItemPreparation>
    {
        readonly ConsumeContext _context;
        readonly ILogger<RequestOrderItemPreparationActivity> _logger;

        public RequestOrderItemPreparationActivity(ConsumeContext context, ILogger<RequestOrderItemPreparationActivity> logger)
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

        public async Task Execute(BehaviorContext<OrderItemState, RequestOrderItemPreparation> context,
            Behavior<OrderItemState, RequestOrderItemPreparation> next)
        {
            _logger.LogInformation($"Order Item State Machine - {nameof(RequestOrderItemPreparationActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;
            context.Instance.OrderId = context.Data.OrderId;

            await _context.Publish<PrepareOrderItem>(
                new
                {
                    context.Data.OrderId,
                    context.Data.OrderItemId,
                    context.Data.RestaurantId,
                    context.Data.MenuItemId
                });
            
            _logger.LogInformation($"Published - {nameof(PrepareOrderItem)}");
            _logger.LogInformation($"Order Item ID - {context.Data.OrderItemId}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, RequestOrderItemPreparation, TException> context,
            Behavior<OrderItemState, RequestOrderItemPreparation> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
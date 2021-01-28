namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using GreenPipes;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class OrderItemsPreparedActivity :
        Activity<OrderState, OrderItemPrepared>
    {
        readonly ILogger<OrderItemsPreparedActivity> _logger;
        readonly IOrderProcessor _client;

        public OrderItemsPreparedActivity(IGrpcClient<IOrderProcessor> grpcClient, ILogger<OrderItemsPreparedActivity> logger)
        {
            _logger = logger;
            _client = grpcClient.Client;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderItemPrepared> context,
            Behavior<OrderState, OrderItemPrepared> next)
        {
            _logger.LogInformation($"Order State Machine - {nameof(OrderItemsPreparedActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;

            var updateResult = await _client.UpdateExpectedOrderItem(
                new ()
                {
                    OrderItemId = context.Data.OrderItemId,
                    Status = OrderItemStatus.Prepared
                });
            
            var result = await _client.GetIncludedOrderItemCount(
                new ()
                {
                    OrderId = context.Instance.CorrelationId,
                    Status = OrderItemStatus.Prepared
                });

            int preparedItemCount = result.Value;
            
            _logger.LogInformation($"PreparedItemCount = {preparedItemCount}");
            
            context.Instance.PreparedItemCount = preparedItemCount;

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderState, OrderItemPrepared, TException> context,
            Behavior<OrderState, OrderItemPrepared> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
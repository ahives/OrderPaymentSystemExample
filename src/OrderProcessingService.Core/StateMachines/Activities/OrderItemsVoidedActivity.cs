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

    public class OrderItemsVoidedActivity :
        Activity<OrderState, OrderItemVoided>
    {
        readonly ILogger<OrderItemsVoidedActivity> _logger;
        readonly IOrderProcessor _client;

        public OrderItemsVoidedActivity(IGrpcClient<IOrderProcessor> grpcClient, ILogger<OrderItemsVoidedActivity> logger)
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

        public async Task Execute(BehaviorContext<OrderState, OrderItemVoided> context,
            Behavior<OrderState, OrderItemVoided> next)
        {
            _logger.LogInformation($"Order State Machine - {nameof(OrderItemsVoidedActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;

            context.Instance.ExpectedItemCount = await GetExpectedOrderItemCount(context.Data.OrderId, OrderItemStatus.Voided);
            context.Instance.PreparedItemCount = await GetOrderItemCount(context.Data.OrderId, OrderItemStatus.Prepared);
            context.Instance.CanceledItemCount = await GetOrderItemCount(context.Data.OrderId, OrderItemStatus.Canceled);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderItemVoided, TException> context,
            Behavior<OrderState, OrderItemVoided> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }

        async Task<int> GetExpectedOrderItemCount(Guid orderId, OrderItemStatus status)
        {
            var result = await _client.GetExcludedOrderItemCount(
                new()
                {
                    OrderId = orderId,
                    Status = status
                });

            return result.Value;
        }

        async Task<int> GetOrderItemCount(Guid orderId, OrderItemStatus status)
        {
            var result = await _client.GetIncludedOrderItemCount(
                new()
                {
                    OrderId = orderId,
                    Status = status
                });

            return result.Value;
        }
    }
}
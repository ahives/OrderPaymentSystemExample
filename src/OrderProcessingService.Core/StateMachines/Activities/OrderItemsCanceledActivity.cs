namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using GreenPipes;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class OrderItemsCanceledActivity :
        Activity<OrderState, OrderItemCanceled>
    {
        readonly ConsumeContext _context;
        readonly ILogger<OrderItemsCanceledActivity> _logger;
        readonly IOrderProcessor _client;

        public OrderItemsCanceledActivity(ConsumeContext context, IGrpcClient<IOrderProcessor> grpcClient, ILogger<OrderItemsCanceledActivity> logger)
        {
            _context = context;
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

        public async Task Execute(BehaviorContext<OrderState, OrderItemCanceled> context,
            Behavior<OrderState, OrderItemCanceled> next)
        {
            _logger.LogInformation($"Order State Machine - {nameof(OrderItemsCanceledActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;
            
            var result = await _client.GetIncludedOrderItemCount(
                new ()
                {
                    OrderId = context.Instance.CorrelationId,
                    Status = OrderItemStatus.Canceled
                });

            int canceledItemCount = result.Value;
            
            _logger.LogInformation($"CanceledItemCount = {canceledItemCount}");
            
            context.Instance.CanceledItemCount = canceledItemCount;

            if (context.Instance.CanceledItemCount == context.Instance.ExpectedItemCount)
            {
                await _context.Publish<CancelOrder>(
                    new()
                    {
                        OrderId = context.Data.OrderId,
                        CourierId = context.Instance.CourierId,
                        CustomerId = context.Instance.CustomerId,
                        RestaurantId = context.Instance.RestaurantId
                    });

                _logger.LogInformation($"Published - {nameof(CancelOrder)}");
            }
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderState, OrderItemCanceled, TException> context,
            Behavior<OrderState, OrderItemCanceled> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
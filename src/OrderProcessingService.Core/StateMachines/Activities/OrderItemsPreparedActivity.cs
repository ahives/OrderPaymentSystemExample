namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class OrderItemsPreparedActivity :
        Activity<OrderState, OrderItemPrepared>
    {
        readonly IOrderProcessor _client;

        public OrderItemsPreparedActivity(IGrpcClient<IOrderProcessor> grpcClient)
        {
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
            Log.Information($"Order State Machine - {nameof(OrderItemsPreparedActivity)} (state = {context.Instance.CurrentState})");

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
            
            Log.Information($"PreparedItemCount = {preparedItemCount}");
            
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
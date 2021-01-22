namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class OrderItemsPreparedActivity :
        Activity<OrderState, OrderItemPrepared>
    {
        readonly IGrpcClient<IOrderProcessor> _client;

        public OrderItemsPreparedActivity(IGrpcClient<IOrderProcessor> client)
        {
            _client = client;
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

            var updateResult = await _client.Client.UpdateExpectedOrderItem(
                new ()
                {
                    OrderItemId = context.Data.OrderItemId,
                    Status = context.Data.Status
                });
            
            var result = await _client.Client.GetExpectedOrderItemCount(
                new ()
                {
                    OrderId = context.Instance.CorrelationId
                });
            
            Log.Information($"ActualItemCount={result.Value}");
            
            context.Instance.ActualItemCount = result.Value;

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
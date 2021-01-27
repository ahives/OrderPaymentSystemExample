namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class OrderCancelRequestActivity :
        Activity<OrderState, OrderCancelRequest>
    {
        readonly ConsumeContext _context;
        readonly IGrpcClient<IOrderProcessor> _client;

        public OrderCancelRequestActivity(ConsumeContext context, IGrpcClient<IOrderProcessor> client)
        {
            _context = context;
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

        public async Task Execute(BehaviorContext<OrderState, OrderCancelRequest> context,
            Behavior<OrderState, OrderCancelRequest> next)
        {
            Log.Information($"Order State Machine - {nameof(OrderCancelRequestActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;

            var result = await _client.Client.GetExpectedOrderItems(new ()
            {
                OrderId = context.Data.OrderId
            });

            foreach (var item in result.Value)
            {
                await _context.Publish<OrderItemCancelRequest>(new ()
                {
                    OrderId = item.OrderId,
                    OrderItemId = item.OrderItemId,
                });
            }
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderState, OrderCancelRequest, TException> context,
            Behavior<OrderState, OrderCancelRequest> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
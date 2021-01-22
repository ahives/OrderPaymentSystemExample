namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using GreenPipes;
    using Sagas;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class OrderCanceledActivity :
        Activity<OrderState, OrderCanceled>
    {
        readonly IGrpcClient<IOrderProcessor> _client;

        public OrderCanceledActivity(IGrpcClient<IOrderProcessor> client)
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

        public async Task Execute(BehaviorContext<OrderState, OrderCanceled> context,
            Behavior<OrderState, OrderCanceled> next)
        {
            Log.Information($"Order State Machine - {nameof(OrderCanceledActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;

            // var items = _db.ExpectedOrderItems
            //     .Where(x => x.OrderId == context.Data.OrderId)
            //     .ToList();
            //
            // foreach (var item in items)
            // {
            //     item.Status = (int) OrderItemStatus.Canceled;
            //     _db.ExpectedOrderItems.Update(item);
            // }
            //
            // int changes = await _db.SaveChangesAsync();
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderCanceled, TException> context,
            Behavior<OrderState, OrderCanceled> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
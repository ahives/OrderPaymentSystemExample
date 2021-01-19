namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class PrepareOrderItemRequestActivity :
        Activity<OrderItemState, PrepareOrderItemRequested>
    {
        readonly ConsumeContext _context;

        public PrepareOrderItemRequestActivity(ConsumeContext context)
        {
            _context = context;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderItemState, PrepareOrderItemRequested> context,
            Behavior<OrderItemState, PrepareOrderItemRequested> next)
        {
            Log.Information($"Order Item State Machine - {nameof(PrepareOrderItemRequestActivity)}");

            context.Instance.Timestamp = DateTime.Now;
            context.Instance.OrderId = context.Data.OrderId;
            
            await _context.Publish<PrepareOrderItem>(new
            {
                context.Data.OrderId,
                context.Data.OrderItemId,
                context.Data.RestaurantId,
                context.Data.MenuItemId
            });
            
            Log.Information($"Published - {nameof(PrepareOrderItem)}");
            Log.Information($"Order Item ID - {context.Data.OrderItemId}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderItemState, PrepareOrderItemRequested, TException> context,
            Behavior<OrderItemState, PrepareOrderItemRequested> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
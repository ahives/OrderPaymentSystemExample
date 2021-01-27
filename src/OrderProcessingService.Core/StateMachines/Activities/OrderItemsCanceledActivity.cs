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

    public class OrderItemsCanceledActivity :
        Activity<OrderState, OrderItemCanceled>
    {
        readonly ConsumeContext _context;

        public OrderItemsCanceledActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<OrderState, OrderItemCanceled> context,
            Behavior<OrderState, OrderItemCanceled> next)
        {
            Log.Information($"Order State Machine - {nameof(OrderItemsCanceledActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;

            if (context.Instance.CanceledItemCount == context.Instance.ExpectedItemCount)
            {
                await _context.Publish<CancelOrder>(new()
                {
                    OrderId = context.Data.OrderId,
                    CourierId = context.Instance.CourierId,
                    CustomerId = context.Instance.CustomerId,
                    RestaurantId = context.Instance.RestaurantId
                });
            }
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderState, OrderItemCanceled, TException> context,
            Behavior<OrderState, OrderItemCanceled> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class PrepareOrderRequestedActivity :
        Activity<OrderState, PrepareOrder>
    {
        readonly ConsumeContext _context;
        readonly OrderProcessingServiceDbContext _db;

        public PrepareOrderRequestedActivity(ConsumeContext context, OrderProcessingServiceDbContext db)
        {
            _context = context;
            _db = db;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, PrepareOrder> context,
            Behavior<OrderState, PrepareOrder> next)
        {
            Log.Information($"Courier State Machine - {nameof(PrepareOrderRequestedActivity)}");
            
            context.Instance.CustomerId = context.Data.CustomerId;
            context.Instance.RestaurantId = context.Data.RestaurantId;
            context.Instance.ExpectedItemCount = context.Data.Items.Length;
            context.Instance.ActualItemCount = 0;
            // context.Instance.Items = items;
            context.Instance.Timestamp = DateTime.Now;
            
            var items = context.Data.Items.MapExpectedOrderItems(context.Data.OrderId);

            await _db.ExpectedOrderItems.AddRangeAsync(items);
            int addCount = await _db.SaveChangesAsync();
            
            // fork each item in order to be prepared
            for (int i = 0; i < context.Data.Items.Length; i++)
            {
                await _context.Publish<PrepareOrderItemRequested>(new
                {
                    context.Data.OrderId,
                    context.Data.RestaurantId,
                    context.Data.Items[i].MenuItemId,
                    context.Data.Items[i].SpecialInstructions
                });
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, PrepareOrder, TException> context,
            Behavior<OrderState, PrepareOrder> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using GreenPipes;
    using Microsoft.EntityFrameworkCore;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class OrderItemsBeingPreparedActivity :
        Activity<OrderState, OrderItemPrepared>
    {
        readonly OrderProcessingServiceDbContext _dbContext;

        public OrderItemsBeingPreparedActivity(OrderProcessingServiceDbContext dbContext)
        {
            _dbContext = dbContext;
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
            Log.Information($"Order State Machine - {nameof(OrderItemsBeingPreparedActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;

            // _dbContext.Orders.Include(x => x.Items);

            for (int i = 0; i < context.Instance.Items.Count; i++)
            {
                if (context.Instance.Items[i].OrderItemId != context.Data.OrderItemId)
                    continue;

                context.Instance.Items[i].Status = context.Data.Status;
                
                // _dbContext.ExpectedOrderItems.Update(context.Instance.Items[i]);
                break;
            }

            // await _dbContext.SaveChangesAsync();

            context.Instance.ActualItemCount = context.Instance.Items
                .Count(x => x.Status == (int) OrderItemStatus.Prepared);

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
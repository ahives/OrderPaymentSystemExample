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
        readonly OrderProcessingServiceDbContext _db;

        public OrderItemsBeingPreparedActivity(OrderProcessingServiceDbContext db)
        {
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

        public async Task Execute(BehaviorContext<OrderState, OrderItemPrepared> context,
            Behavior<OrderState, OrderItemPrepared> next)
        {
            Log.Information($"Courier State Machine - {nameof(OrderItemsBeingPreparedActivity)}");
            
            var items = _db.ExpectedOrderItems
                .Where(x => x.OrderId == context.Data.OrderId);

            var item = await items.FirstOrDefaultAsync(x => x.CorrelationId == context.Data.OrderItemId);

            if (item != null)
            {
                item.Status = context.Data.Status;
                item.Timestamp = context.Data.Timestamp;

                _db.Update(item);
            }

            // for (int i = 0; i < items.Count; i++)
            // {
            //     if (items[i].CorrelationId != context.Data.OrderItemId)
            //         continue;
            //
            //     items[i].Status = context.Data.Status;
            //     items[i].Timestamp = context.Data.Timestamp;
            //     break;
            // }

            int changes = await _db.SaveChangesAsync();

            context.Instance.ActualItemCount = items
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
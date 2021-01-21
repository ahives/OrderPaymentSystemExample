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
    using Services.Core.Events;

    public class OrderItemsPreparedActivity :
        Activity<OrderState, OrderItemPrepared>
    {
        readonly OrderProcessingServiceDbContext _dbContext;

        public OrderItemsPreparedActivity(OrderProcessingServiceDbContext dbContext)
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
            Log.Information($"Order State Machine - {nameof(OrderItemsPreparedActivity)} (state = {context.Instance.CurrentState})");

            context.Instance.Timestamp = DateTime.Now;

            var item = await _dbContext.ExpectedOrderItems.FindAsync(context.Data.OrderItemId);
            
            if (item != null)
            {
                item.Status = context.Data.Status;
                
                _dbContext.ExpectedOrderItems.Update(item);
                
                await _dbContext.SaveChangesAsync();
            }
            
            context.Instance.ActualItemCount = _dbContext.ExpectedOrderItems
                .Where(x => x.OrderId == context.Instance.CorrelationId)
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
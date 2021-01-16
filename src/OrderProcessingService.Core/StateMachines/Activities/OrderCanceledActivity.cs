namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class OrderCanceledActivity :
        Activity<OrderState, OrderCanceled>
    {
        readonly OrderProcessingServiceDbContext _db;

        public OrderCanceledActivity(OrderProcessingServiceDbContext db)
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

        public async Task Execute(BehaviorContext<OrderState, OrderCanceled> context,
            Behavior<OrderState, OrderCanceled> next)
        {
            var items = _db.ExpectedOrderItems
                .Where(x => x.OrderId == context.Data.OrderId)
                .ToList();
            
            foreach (var item in items)
            {
                item.Status = (int) OrderItemStatus.Canceled;
                _db.ExpectedOrderItems.Update(item);
            }

            int changes = await _db.SaveChangesAsync();
            
            context.Instance.Timestamp = DateTime.Now;
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderCanceled, TException> context,
            Behavior<OrderState, OrderCanceled> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
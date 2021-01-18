namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Serilog;
    using Services.Core.Events;
    using Services.Core.Model;

    public class PrepareOrderRequestedActivity :
        Activity<OrderState, PrepareOrderRequest>
    {
        readonly ConsumeContext _context;

        public PrepareOrderRequestedActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<OrderState, PrepareOrderRequest> context,
            Behavior<OrderState, PrepareOrderRequest> next)
        {
            Log.Information($"Order Item State Machine - {nameof(PrepareOrderRequestedActivity)}");
            
            var items = GenerateOrderItemIdentifiers(context.Data.Items);

            context.Instance.Timestamp = DateTime.Now;
            context.Instance.CustomerId = context.Data.CustomerId;
            context.Instance.RestaurantId = context.Data.RestaurantId;
            context.Instance.ExpectedItemCount = context.Data.Items.Length;
            context.Instance.ActualItemCount = 0;
            context.Instance.Items = MapExpectedOrderItems(items, context.Instance.CorrelationId).ToList();
            
            await _context.Publish<PrepareOrder>(new
            {
                OrderId = context.Instance.CorrelationId,
                context.Data.CustomerId,
                context.Data.RestaurantId,
                context.Data.AddressId,
                Items = items.ToArray()
            });
            
            Log.Information($"Published - {nameof(PrepareOrder)}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, PrepareOrderRequest, TException> context,
            Behavior<OrderState, PrepareOrderRequest> next) where TException : Exception
        {
            await next.Faulted(context);
        }
        
        IEnumerable<Item> GenerateOrderItemIdentifiers(Item[] items) =>
            items.Select(t => new Item
            {
                OrderItemId = NewId.NextGuid(),
                MenuItemId = t.MenuItemId,
                Status = t.Status,
                SpecialInstructions = t.SpecialInstructions
            });
        
        IEnumerable<ExpectedOrderItem> MapExpectedOrderItems(IEnumerable<Item> items, Guid orderId) =>
            items.Select(t => new ExpectedOrderItem
            {
                CorrelationId = t.OrderItemId,
                OrderId = orderId,
                Status = t.Status,
                Timestamp = DateTime.Now
            });
    }
}
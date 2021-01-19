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
            
            var expectedOrderItems = MapExpectedOrderItems(items, context.Instance.CorrelationId).ToList();
            
            context.Instance.Items = expectedOrderItems;

            var publishedItems = items.ToArray();
            
            await _context.Publish<PrepareOrder>(new
            {
                OrderId = context.Instance.CorrelationId,
                context.Data.CustomerId,
                context.Data.RestaurantId,
                context.Data.AddressId,
                Items = publishedItems
            });
            
            Log.Information($"Published - {nameof(PrepareOrder)}");

            for (int i = 0; i < publishedItems.Length; i++)
            {
                Log.Information($"Order Item ID => {publishedItems[i].OrderItemId} (published), {expectedOrderItems[i].OrderItemId} (state)");
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, PrepareOrderRequest, TException> context,
            Behavior<OrderState, PrepareOrderRequest> next) where TException : Exception
        {
            await next.Faulted(context);
        }
        
        IEnumerable<Item> GenerateOrderItemIdentifiers(Item[] items) =>
            items.Select(x => new Item
            {
                OrderItemId = NewId.NextGuid(),
                MenuItemId = x.MenuItemId,
                Status = x.Status,
                SpecialInstructions = x.SpecialInstructions
            });
        
        IEnumerable<ExpectedOrderItem> MapExpectedOrderItems(IEnumerable<Item> items, Guid orderId) =>
            items.Select(x => new ExpectedOrderItem
            {
                CorrelationId = NewId.NextGuid(),
                OrderItemId = x.OrderItemId,
                OrderId = orderId,
                Status = x.Status,
                Timestamp = DateTime.Now
            });
    }
}
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
        Activity<OrderState, RequestOrderPreparation>
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

        public async Task Execute(BehaviorContext<OrderState, RequestOrderPreparation> context,
            Behavior<OrderState, RequestOrderPreparation> next)
        {
            Log.Information($"Order State Machine - {nameof(PrepareOrderRequestedActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.CustomerId = context.Data.CustomerId;
            context.Instance.RestaurantId = context.Data.RestaurantId;
            context.Instance.ExpectedItemCount = context.Data.Items.Length;
            context.Instance.ActualItemCount = 0;
            
            var items = GenerateOrderItemIdentifiers(context.Data.Items).ToList();
            
            context.Instance.Items = MapExpectedOrderItems(items, context.Instance.CorrelationId).ToList();
            
            await _context.Publish<PrepareOrder>(new
            {
                OrderId = context.Instance.CorrelationId,
                context.Data.CustomerId,
                context.Data.RestaurantId,
                context.Data.AddressId,
                Items = items
            });
            
            Log.Information($"Published - {nameof(PrepareOrder)}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, RequestOrderPreparation, TException> context,
            Behavior<OrderState, RequestOrderPreparation> next) where TException : Exception
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
        
        IEnumerable<ExpectedOrderItem> MapExpectedOrderItems(List<Item> items, Guid orderId) =>
            items.Select(x => new ExpectedOrderItem
            {
                OrderItemId = x.OrderItemId,
                OrderId = orderId,
                Status = x.Status,
            });
    }
}
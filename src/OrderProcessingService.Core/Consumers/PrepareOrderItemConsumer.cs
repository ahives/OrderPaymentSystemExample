namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Service.Grpc.Core;
    using Service.Grpc.Core.Model;
    using Services.Core.Events;

    public class PrepareOrderItemConsumer :
        IConsumer<PrepareOrderItem>
    {
        readonly IGrpcClient<IOrderProcessor> _processorClient;
        readonly IShelfManager _manager;

        public PrepareOrderItemConsumer(IGrpcClient<IOrderProcessor> processorClient, IShelfManager manager)
        {
            _processorClient = processorClient;
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<PrepareOrderItem> context)
        {
            var result = await _processorClient.Client.PrepareItem(new ()
            {
                OrderId = context.Message.OrderId,
                OrderItemId = NewId.NextGuid(),
                MenuItemId = context.Message.MenuItemId,
                SpecialInstructions = context.Message.SpecialInstructions
            });
            
            if (result.IsSuccessful)
            {
                await MoveToShelf(context, result.Value);
            }
            else
            {
                await context.Publish<OrderItemNotPrepared>(new
                {
                    context.Message.OrderId,
                    result.Value.OrderItemId,
                    result.Value.Status,
                    result.Value.ShelfId
                });
            }
        }

        async Task MoveToShelf(ConsumeContext<PrepareOrderItem> context, OrderItem orderItem)
        {
            var result = await _manager.MoveToShelf(new ShelfManagerRequest
            {
                OrderItemId = orderItem.OrderItemId,
                RestaurantId = context.Message.RestaurantId,
                MenuItemId = context.Message.MenuItemId
            });
                
            if (result.IsSuccessful)
            {
                // TODO: move to appropriate shelf
                await context.Publish<OrderItemPrepared>(new
                {
                    context.Message.OrderId,
                    orderItem.OrderItemId,
                    orderItem.Status,
                    orderItem.ShelfId
                });
            }
            else
            {
                await MoveToOverflow(context, orderItem);
            }
        }

        async Task MoveToOverflow(ConsumeContext<PrepareOrderItem> context, OrderItem orderItem)
        {
            var result = await _manager.MoveToOverflow(new ShelfManagerRequest
            {
                OrderItemId = orderItem.OrderItemId,
                MenuItemId = context.Message.MenuItemId
            });

            if (result.IsSuccessful)
            {
                await context.Publish<OrderItemPrepared>(new
                {
                    context.Message.OrderId,
                    orderItem.OrderItemId,
                    orderItem.Status,
                    orderItem.ShelfId
                });
            }
            else
            {
                await context.Publish<OrderItemNotPrepared>(new
                {
                    context.Message.OrderId,
                    orderItem.OrderItemId,
                    orderItem.Status,
                    orderItem.ShelfId
                });
            }
        }
    }
}
namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class PrepareOrderItemConsumer :
        IConsumer<PrepareOrderItem>
    {
        readonly IOrderProcessor _client;

        public PrepareOrderItemConsumer(IGrpcClient<IOrderProcessor> grpcClient)
        {
            _client = grpcClient.Client;
        }

        public async Task Consume(ConsumeContext<PrepareOrderItem> context)
        {
            Log.Information($"Consumer - {nameof(PrepareOrderItemConsumer)} => consumed {nameof(PrepareOrderItem)} event");

            var result = await _client.AddNewOrderItem(
                new()
                {
                    OrderId = context.Message.OrderId,
                    OrderItemId = context.Message.OrderItemId,
                    MenuItemId = context.Message.MenuItemId,
                    SpecialInstructions = context.Message.SpecialInstructions
                });
            
            if (result.IsSuccessful)
            {
                await context.Publish<OrderItemPrepared>(
                    new
                    {
                        context.Message.OrderId,
                        result.Value.OrderItemId,
                        result.Value.Status,
                        result.Value.ShelfId
                    });
                
                Log.Information($"Published - {nameof(OrderItemPrepared)}");
            }
            else
            {
                await context.Publish<OrderItemNotPrepared>(
                    new
                    {
                        context.Message.OrderId,
                        result.Value.OrderItemId,
                        result.Value.Status,
                        result.Value.ShelfId
                    });
                
                Log.Information($"Published - {nameof(OrderItemNotPrepared)}");
            }
        }

        // async Task MoveToShelf(ConsumeContext<PrepareOrderItem> context, OrderItem orderItem)
        // {
        //     var result = await _manager.MoveToShelf(new ShelfManagerRequest
        //     {
        //         OrderItemId = orderItem.OrderItemId,
        //         RestaurantId = context.Message.RestaurantId,
        //         MenuItemId = context.Message.MenuItemId
        //     });
        //         
        //     if (result.IsSuccessful)
        //     {
        //         // TODO: move to appropriate shelf
        //         await context.Publish<OrderItemPrepared>(new
        //         {
        //             context.Message.OrderId,
        //             orderItem.OrderItemId,
        //             orderItem.Status,
        //             orderItem.ShelfId
        //         });
        //     }
        //     else
        //     {
        //         await MoveToOverflow(context, orderItem);
        //     }
        // }
        //
        // async Task MoveToOverflow(ConsumeContext<PrepareOrderItem> context, OrderItem orderItem)
        // {
        //     var result = await _manager.MoveToOverflow(new ShelfManagerRequest
        //     {
        //         OrderItemId = orderItem.OrderItemId,
        //         MenuItemId = context.Message.MenuItemId
        //     });
        //
        //     if (result.IsSuccessful)
        //     {
        //         await context.Publish<OrderItemPrepared>(new
        //         {
        //             context.Message.OrderId,
        //             orderItem.OrderItemId,
        //             orderItem.Status,
        //             orderItem.ShelfId
        //         });
        //     }
        //     else
        //     {
        //         await context.Publish<OrderItemNotPrepared>(new
        //         {
        //             context.Message.OrderId,
        //             orderItem.OrderItemId,
        //             orderItem.Status,
        //             orderItem.ShelfId
        //         });
        //     }
        // }
    }
}
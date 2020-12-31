namespace RestaurantService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core;
    using Services.Core.Events;

    public class PrepareOrderItemConsumer :
        IConsumer<PrepareOrderItem>
    {
        readonly IPrepareOrder _prepareOrder;
        readonly IKitchenManager _manager;

        public PrepareOrderItemConsumer(IPrepareOrder prepareOrder, IKitchenManager manager)
        {
            _prepareOrder = prepareOrder;
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<PrepareOrderItem> context)
        {
            var result = await _prepareOrder.Prepare(new ()
            {
                OrderId = context.Message.OrderId,
                OrderItemId = NewId.NextGuid(),
                MenuItemId = context.Message.MenuItemId,
                SpecialInstructions = context.Message.SpecialInstructions
            });
            
            if (result.IsSuccessful)
            {
                var moveResult = await _manager.MoveToShelf(new ShelfMoveCriteria
                {
                    OrderItemId = result.Value.OrderItemId,
                    MenuItemId = context.Message.MenuItemId
                });
                
                if (moveResult.IsSuccessful)
                {
                    // TODO: move to appropriate shelf
                    await context.Publish<OrderItemPrepared>(new
                    {
                        context.Message.OrderId,
                        result.Value.OrderItemId,
                        result.Value.Status,
                        // result.Shelf.ShelfId
                    });
                }
            }
            else
            {
                // attempt to move to an overflow shelf
                
                // await context.Publish<OrderItemPrepared>(new
                // {
                //     context.Message.OrderId
                // });
            }
        }
    }
}
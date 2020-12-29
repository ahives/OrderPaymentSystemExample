namespace RestaurantService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core;
    using Services.Core.Events;

    public class PrepareOrderItemConsumer :
        IConsumer<PrepareOrderItem>
    {
        readonly IOrderManager _manager;

        public PrepareOrderItemConsumer(IOrderManager manager)
        {
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<PrepareOrderItem> context)
        {
            var result = await _manager.Prepare(context.Message);
            
            if (result.OperationPerformed == OperationType.MovedToShelf)
            {
                await context.Publish<OrderItemPrepared>(new
                {
                    context.Message.OrderId,
                    context.Message.OrderItemId,
                    // result.Shelf.ShelfId
                });
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
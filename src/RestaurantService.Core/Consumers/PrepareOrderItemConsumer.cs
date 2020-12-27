namespace RestaurantService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
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
            var result = await _manager.Prepare(new OperationContext<OrderItemStatusPayload>
            {
                Payload = new OrderItemStatusPayload
                {
                    OrderId = context.Message.OrderId,
                    Status = OrderItemStatus.Prepared,
                    ShelfId = 0
                }
            });

            await context.Publish<OrderItemPrepared>(new
            {
                context.Message.OrderId
            });
        }
    }
}
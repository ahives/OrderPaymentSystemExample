namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Services.Core.Events;

    public class OrderDeliveryConsumer :
        IConsumer<DeliverOrder>
    {
        public async Task Consume(ConsumeContext<DeliverOrder> context)
        {
            Log.Information($"Courier State Machine - {nameof(OrderDeliveryConsumer)}");
            
            await context.Publish<OrderDelivered>(new
            {
                context.Message.CourierId,
                context.Message.OrderId,
                context.Message.CustomerId,
                context.Message.RestaurantId
            });
        }
    }
}
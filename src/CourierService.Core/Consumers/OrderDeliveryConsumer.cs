namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class OrderDeliveryConsumer :
        IConsumer<DeliverOrder>
    {
        readonly ICourierDispatcherClient _client;

        public OrderDeliveryConsumer(ICourierDispatcherClient client)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<DeliverOrder> context)
        {
            Log.Information($"Consumer - {nameof(OrderDeliveryConsumer)} => consumed {nameof(DeliverOrder)} event");

            var result = await _client.Client.Deliver(new CourierDispatchRequest()
            {
                CourierId = context.Message.CourierId
            });
            
            if (result.IsSuccessful)
            {
                await context.Publish<OrderDelivered>(new
                {
                    context.Message.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
                
                Log.Information($"Published - {nameof(OrderDelivered)}");
            }
            else
            {
                var statusResult = await _client.Client.ChangeStatus(new CourierStatusChangeRequest()
                {
                    CourierId = context.Message.CourierId,
                    Status = CourierStatus.ArrivedAtCustomer
                });

                if (!statusResult.IsSuccessful)
                {
                    // TODO: fault the consumer and retry
                }
            }
        }
    }
}
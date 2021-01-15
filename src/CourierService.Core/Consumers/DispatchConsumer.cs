namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class DispatchConsumer :
        IConsumer<DispatchCourier>
    {
        readonly IGrpcClient<ICourierDispatcher> _client;

        public DispatchConsumer(IGrpcClient<ICourierDispatcher> client)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<DispatchCourier> context)
        {
            Log.Information($"Consumer - {nameof(DispatchConsumer)} => consumed {nameof(DispatchCourier)} event");
            
            var result = await _client.Client.ChangeStatus(new ()
            {
                CourierId = context.Message.CourierId,
                Status = CourierStatus.Dispatched
            });
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatched>(new
                {
                    result.Value.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
                
                Log.Information($"Published - {nameof(CourierDispatched)}");
            }
            else
            {
                await context.Publish<CourierNotDispatched>(new
                {
                    result.Value.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
                
                Log.Information($"Published - {nameof(CourierNotDispatched)}");
            }
        }
    }
}
namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class DispatchIdentificationConsumer :
        IConsumer<IdentifyCourierForDispatch>
    {
        readonly IGrpcClient<ICourierDispatcher> _client;

        public DispatchIdentificationConsumer(IGrpcClient<ICourierDispatcher> client)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<IdentifyCourierForDispatch> context)
        {
            Log.Information($"Consumer - {nameof(DispatchIdentificationConsumer)} => consumed {nameof(IdentifyCourierForDispatch)} event");

            var result = await _client.Client.Identify(new () {CustomerId = context.Message.CustomerId});

            if (result.IsSuccessful)
            {
                await context.Publish<CourierIdentifiedForDispatch>(new()
                {
                    CourierId = result.Value.CourierId,
                    OrderId = context.Message.OrderId,
                    RestaurantId = context.Message.RestaurantId,
                    CustomerId = context.Message.CustomerId
                });
                
                Log.Information($"Identified courier - {result.Value.CourierId}");
                Log.Information($"Published - {nameof(CourierIdentifiedForDispatch)}");
            }
            else
            {
                await context.Publish<CourierNotIdentifiedForDispatch>(new()
                {
                    OrderId = context.Message.OrderId,
                    RestaurantId = context.Message.RestaurantId,
                    CustomerId = context.Message.CustomerId
                });
                
                Log.Information($"Published - {nameof(CourierNotIdentifiedForDispatch)}");
            }
        }
    }
}
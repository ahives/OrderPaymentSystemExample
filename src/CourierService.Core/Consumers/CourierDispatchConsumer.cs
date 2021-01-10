namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Service.Grpc.Core.Model;
    using Services.Core.Events;

    public class CourierDispatchConsumer :
        IConsumer<DispatchCourier>
    {
        readonly ICourierDispatcherClient _client;

        public CourierDispatchConsumer(ICourierDispatcherClient client)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<DispatchCourier> context)
        {
            Log.Information($"Consumer - {nameof(CourierDispatchConsumer)}");
            
            Result<Courier> result = await _client.Client.Dispatch(new ()
            {
                Street = context.Message.Street,
                City = context.Message.City,
                RegionId = context.Message.RegionId,
                ZipCode = context.Message.ZipCode
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
                
                Log.Information($"Courier {result.Value.CourierId} was successfully dispatched.");
            }
        }
    }
}
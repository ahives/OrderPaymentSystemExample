namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class EnRouteToRestaurantConsumer :
        IConsumer<CourierEnRouteToRestaurant>
    {
        readonly ICourierDispatcherClient _client;

        public EnRouteToRestaurantConsumer(ICourierDispatcherClient client)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<CourierEnRouteToRestaurant> context)
        {
            Log.Information($"Consumer - {nameof(EnRouteToRestaurantConsumer)} => consumed {nameof(CourierEnRouteToRestaurant)} event");

            var result = await _client.Client.ChangeStatus(new ()
            {
                CourierId = context.Message.CourierId,
                Status = CourierStatus.EnRouteToRestaurant
            });
        }
    }
}
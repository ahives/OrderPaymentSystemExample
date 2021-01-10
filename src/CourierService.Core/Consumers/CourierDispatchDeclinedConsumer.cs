namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class CourierDispatchDeclinedConsumer :
        IConsumer<DeclineCourierDispatch>
    {
        readonly ICourierDispatcherClient _client;

        public CourierDispatchDeclinedConsumer(ICourierDispatcherClient client)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<DeclineCourierDispatch> context)
        {
            Log.Information($"Consumer - {nameof(CourierDispatchDeclinedConsumer)}");
            
            var result = await _client.Client.Decline(new () {CourierId = context.Message.CourierId});
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatchDeclined>(new
                {
                    context.Message.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
                
                Log.Information($"Courier {result.Value.CourierId} ({result.Value.FirstName} {result.Value.LastName}) declined dispatch.");
            }
        }
    }
}
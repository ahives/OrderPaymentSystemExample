namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class DispatchDeclinedConsumer :
        IConsumer<DeclineCourierDispatch>
    {
        readonly ICourierDispatcher _client;
        readonly ILogger<DispatchDeclinedConsumer> _logger;

        public DispatchDeclinedConsumer(IGrpcClient<ICourierDispatcher> grpcClient, ILogger<DispatchDeclinedConsumer> logger)
        {
            _client = grpcClient.Client;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DeclineCourierDispatch> context)
        {
            _logger.LogInformation($"Consumer - {nameof(DispatchDeclinedConsumer)} => consumed {nameof(DeclineCourierDispatch)} event");
            
            var result = await _client.Decline(new () {CourierId = context.Message.CourierId});
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatchDeclined>(
                    new
                    {
                        context.Message.CourierId,
                        context.Message.OrderId,
                        context.Message.CustomerId,
                        context.Message.RestaurantId
                    });
                
                _logger.LogInformation($"Published - {nameof(CourierDispatchDeclined)}");
            }
        }
    }
}
namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class EnRouteToCustomerConsumer :
        IConsumer<CourierEnRouteToCustomer>
    {
        readonly ICourierDispatcher _client;
        readonly ILogger<EnRouteToCustomerConsumer> _logger;

        public EnRouteToCustomerConsumer(IGrpcClient<ICourierDispatcher> grpcClient, ILogger<EnRouteToCustomerConsumer> logger)
        {
            _client = grpcClient.Client;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CourierEnRouteToCustomer> context)
        {
            _logger.LogInformation($"Consumer - {nameof(EnRouteToCustomerConsumer)} => consumed {nameof(CourierEnRouteToCustomer)} event");

            var result = await _client.ChangeStatus(
                new()
                {
                    CourierId = context.Message.CourierId,
                    Status = CourierStatus.EnRouteToCustomer
                });
        }
    }
}
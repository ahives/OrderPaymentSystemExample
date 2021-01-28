namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class DispatchConfirmationConsumer :
        IConsumer<ConfirmCourierDispatch>
    {
        readonly ICourierDispatcher _client;
        readonly ILogger<DispatchConfirmationConsumer> _logger;

        public DispatchConfirmationConsumer(IGrpcClient<ICourierDispatcher> grpcClient, ILogger<DispatchConfirmationConsumer> logger)
        {
            _client = grpcClient.Client;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ConfirmCourierDispatch> context)
        {
            _logger.LogInformation($"Consumer - {nameof(DispatchConfirmationConsumer)} => consumed {nameof(ConfirmCourierDispatch)} event");

            var result = await _client.ChangeStatus(
                new()
                {
                    CourierId = context.Message.CourierId,
                    Status = CourierStatus.DispatchConfirmed
                });
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatchConfirmed>(
                    new
                    {
                        result.Value.CourierId,
                        context.Message.OrderId,
                        context.Message.CustomerId,
                        context.Message.RestaurantId
                    });
                
                _logger.LogInformation($"Published - {nameof(CourierDispatchConfirmed)}");
            }
            else
            {
                await context.Publish<CourierDispatchDeclined>(
                    new
                    {
                        result.Value.CourierId,
                        context.Message.OrderId,
                        context.Message.CustomerId,
                        context.Message.RestaurantId
                    });
                
                _logger.LogInformation($"Published - {nameof(CourierDispatchDeclined)}");
            }
        }
    }
}
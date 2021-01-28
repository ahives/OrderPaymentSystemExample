namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class DispatchCancellationConsumer :
        IConsumer<CancelCourierDispatch>
    {
        readonly ILogger<DispatchCancellationConsumer> _logger;
        readonly ICourierDispatcher _client;

        public DispatchCancellationConsumer(IGrpcClient<ICourierDispatcher> grpcClient, ILogger<DispatchCancellationConsumer> logger)
        {
            _logger = logger;
            _client = grpcClient.Client;
        }

        public async Task Consume(ConsumeContext<CancelCourierDispatch> context)
        {
            _logger.LogInformation($"Consumer - {nameof(DispatchCancellationConsumer)} => consumed {nameof(CancelCourierDispatch)} event");
            
            var result = await _client.ChangeStatus(new ()
            {
                CourierId = context.Message.CourierId,
                Status = CourierStatus.DispatchCanceled
            });
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatchCanceled>(new
                {
                    result.Value.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
                
                _logger.LogInformation($"Published - {nameof(CourierDispatchCanceled)}");
            }
        }
    }
}
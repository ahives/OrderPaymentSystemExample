namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class DispatchCancellationConsumer :
        IConsumer<CancelCourierDispatch>
    {
        readonly ICourierDispatcher _client;

        public DispatchCancellationConsumer(IGrpcClient<ICourierDispatcher> client)
        {
            _client = client.Client;
        }

        public async Task Consume(ConsumeContext<CancelCourierDispatch> context)
        {
            Log.Information($"Consumer - {nameof(DispatchCancellationConsumer)} => consumed {nameof(CancelCourierDispatch)} event");
            
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
                
                Log.Information($"Published - {nameof(CourierDispatchCanceled)}");
            }
        }
    }
}
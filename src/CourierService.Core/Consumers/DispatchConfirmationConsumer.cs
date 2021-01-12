namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class DispatchConfirmationConsumer :
        IConsumer<ConfirmCourierDispatch>
    {
        readonly ICourierDispatcherClient _client;

        public DispatchConfirmationConsumer(ICourierDispatcherClient client)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<ConfirmCourierDispatch> context)
        {
            Log.Information($"Consumer - {nameof(DispatchConfirmationConsumer)}");
            
            var result = await _client.Client.ChangeStatus(new ()
            {
                CourierId = context.Message.CourierId,
                Status = CourierStatus.Confirmed
            });
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatchConfirmed>(new
                {
                    result.Value.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
                
                Log.Information($"Sent - {nameof(CourierDispatchConfirmed)}");
            }
            else
            {
                await context.Publish<CourierDispatchDeclined>(new
                {
                    result.Value.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
                
                Log.Information($"Sent - {nameof(CourierDispatchDeclined)}");
            }
        }
    }
}
namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core;
    using Services.Core.Events;

    public class CourierDispatchConfirmationConsumer :
        IConsumer<ConfirmCourierDispatch>
    {
        readonly ICourierDispatcher _dispatcher;

        public CourierDispatchConfirmationConsumer(ICourierDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task Consume(ConsumeContext<ConfirmCourierDispatch> context)
        {
            Log.Information($"Consumer - {nameof(CourierDispatchConfirmationConsumer)}");
            
            var result = await _dispatcher.Confirm(new () {CourierId = context.Message.CourierId});
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatchConfirmed>(new
                {
                    result.Value.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
                
                Log.Information($"Courier {result.Value.CourierId} ({result.Value.FirstName} {result.Value.LastName}) declined dispatch.");
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
                
                Log.Information($"Courier {result.Value.CourierId} ({result.Value.FirstName} {result.Value.LastName}) was declined.");
            }
        }
    }
}
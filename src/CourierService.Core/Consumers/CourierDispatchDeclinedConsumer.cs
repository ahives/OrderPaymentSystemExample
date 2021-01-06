namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Services.Core;
    using Services.Core.Events;

    public class CourierDispatchDeclinedConsumer :
        IConsumer<DeclineCourierDispatch>
    {
        readonly ICourierDispatcher _dispatcher;

        public CourierDispatchDeclinedConsumer(ICourierDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task Consume(ConsumeContext<DeclineCourierDispatch> context)
        {
            Log.Information($"Consumer - {nameof(CourierDispatchDeclinedConsumer)}");
            
            var result = await _dispatcher.Decline(context.Message.CourierId);
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierDispatchDeclined>(new
                {
                    context.Message.CourierId,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
            }
        }
    }
}
namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Serilog;
    using Service.Grpc.Core;
    using Services.Core.Events;

    public class EnRouteToCustomerConsumer :
        IConsumer<CourierEnRouteToCustomer>
    {
        readonly ICourierDispatcherClient _client;

        public EnRouteToCustomerConsumer(ICourierDispatcherClient client)
        {
            _client = client;
        }

        public async Task Consume(ConsumeContext<CourierEnRouteToCustomer> context)
        {
            Log.Information($"Consumer - {nameof(EnRouteToCustomerConsumer)} => consumed {nameof(CourierEnRouteToCustomer)} event");

            var result = await _client.Client.ChangeStatus(new ()
            {
                CourierId = context.Message.CourierId,
                Status = CourierStatus.EnRouteToCustomer
            });
        }
    }
}
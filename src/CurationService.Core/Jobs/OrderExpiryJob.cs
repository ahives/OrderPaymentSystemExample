namespace CurationService.Core.Jobs
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Quartz;
    using Serilog;
    using Services.Core;

    public class OrderExpiryJob :
        IJob
    {
        readonly IPublishEndpoint _endpoint;

        public OrderExpiryJob(IPublishEndpoint endpoint)
        {
            _endpoint = endpoint;
            // IConfiguration configuration = new ConfigurationBuilder()
            //     .AddJsonFile("appsettings.json")
            //     .Build();
            //
            // configuration.GetSection("")
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // await _endpoint.Publish<CurateOrders>(new
            // {
            //     OrderId = NewId.NextGuid(),
            //     CustomerId = NewId.NextGuid(),
            //     RestaurantId = NewId.NextGuid(),
            //     Timestamp = DateTime.Now
            // });

            Log.Information("Order expiry executing");
            
            // var results = _expireOrders.Expire();

            Log.Information("Order expiry executed");
        }
    }
}
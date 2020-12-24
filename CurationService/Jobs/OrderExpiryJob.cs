namespace CurationService.Jobs
{
    using System.Threading.Tasks;
    using MassTransit;
    using Quartz;
    using Restaurant.Core;
    using Serilog;

    public class OrderExpiryJob :
        IJob
    {
        readonly IPublishEndpoint _endpoint;
        readonly IExpireOrders _expireOrders;

        public OrderExpiryJob(IPublishEndpoint endpoint, IExpireOrders expireOrders)
        {
            _endpoint = endpoint;
            _expireOrders = expireOrders;
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
            
            var results = _expireOrders.Expire();

            Log.Information("Order expiry executed");
        }
    }
}
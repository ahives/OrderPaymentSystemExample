namespace CurationService.Core.Jobs
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Quartz;
    using Restaurant.Core.Events;

    public class OrderCurationJob :
        IJob
    {
        readonly IPublishEndpoint _endpoint;

        public OrderCurationJob(IPublishEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _endpoint.Publish<CurateOrders>(new
            {
                OrderId = NewId.NextGuid(),
                CustomerId = NewId.NextGuid(),
                RestaurantId = NewId.NextGuid(),
                Timestamp = DateTime.Now
            });
        }
    }
}
namespace CurationService.Jobs
{
    using System;
    using MassTransit;
    using Quartz;
    using Quartz.Spi;
    using Restaurant.Core;

    public class KitchenMonitorJobFactory :
        IJobFactory
    {
        readonly IPublishEndpoint _endpoint;
        readonly IExpireOrders _expireOrders;

        public KitchenMonitorJobFactory(IPublishEndpoint endpoint, IExpireOrders expireOrders)
        {
            _endpoint = endpoint;
            _expireOrders = expireOrders;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            if (bundle.JobDetail.JobType == typeof(OrderCurationJob))
                return new OrderCurationJob(_endpoint);
            
            if (bundle.JobDetail.JobType == typeof(OrderExpiryJob))
                return new OrderExpiryJob(_endpoint, _expireOrders);

            throw new Exception();
        }

        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}
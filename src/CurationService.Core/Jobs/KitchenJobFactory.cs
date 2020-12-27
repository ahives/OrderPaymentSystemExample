namespace CurationService.Core.Jobs
{
    using System;
    using MassTransit;
    using Quartz;
    using Quartz.Spi;
    using Services.Core;

    public class KitchenJobFactory :
        IJobFactory
    {
        readonly IPublishEndpoint _endpoint;

        public KitchenJobFactory(IPublishEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            if (bundle.JobDetail.JobType == typeof(OrderCurationJob))
                return new OrderCurationJob(_endpoint);
            
            if (bundle.JobDetail.JobType == typeof(OrderExpiryJob))
                return new OrderExpiryJob(_endpoint);

            throw new Exception();
        }

        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}
namespace CurationService
{
    using System;
    using MassTransit;
    using Quartz;
    using Quartz.Spi;

    public class ShelfMonitorJobFactory :
        IJobFactory
    {
        readonly IPublishEndpoint _endpoint;

        public ShelfMonitorJobFactory(IPublishEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) => new ShelfMonitorJob(_endpoint);

        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}
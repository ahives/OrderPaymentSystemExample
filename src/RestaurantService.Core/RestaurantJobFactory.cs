namespace RestaurantService.Core
{
    using System;
    using Quartz;
    using Quartz.Spi;
    using Services.Core;

    public class RestaurantJobFactory :
        IJobFactory
    {
        readonly ILowInventoryDetector _detector;

        public RestaurantJobFactory(ILowInventoryDetector detector)
        {
            _detector = detector;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return new LowInventoryDetectorJob(_detector);
        }

        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}
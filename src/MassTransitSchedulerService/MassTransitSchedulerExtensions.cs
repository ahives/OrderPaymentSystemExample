namespace MassTransitSchedulerService
{
    using System;
    using GreenPipes;
    using MassTransit;
    using MassTransit.QuartzIntegration;
    using MassTransit.QuartzIntegration.Configuration;
    using MassTransit.Scheduling;

    public static class MassTransitSchedulerExtensions
    {
        public static void AddScheduling(this IBusFactoryConfigurator configurator, Action<InMemorySchedulerOptions> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var options = new InMemorySchedulerOptions();
            configure?.Invoke(options);

            if (options.SchedulerFactory == null)
                throw new ArgumentNullException(nameof(options.SchedulerFactory));

            Uri inputAddress = null;

            var observer = new SchedulerBusObserver(options);
            
            configurator.ReceiveEndpoint(options.QueueName, e =>
            {
                var partitioner = configurator.CreatePartitioner(Environment.ProcessorCount);

                e.Consumer(() => new ScheduleMessageConsumer(observer.Scheduler), x =>
                    x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));

                e.Consumer(() => new CancelScheduledMessageConsumer(observer.Scheduler), x =>
                    x.Message<CancelScheduledMessage>(m => m.UsePartitioner(partitioner, p => p.Message.TokenId)));

                configurator.UseMessageScheduler(e.InputAddress);

                configurator.ConnectBusObserver(observer);
            });
        }
    }
}
namespace CourierService.Definitions
{
    using Core.Consumers;
    using MassTransit;
    using MassTransit.ConsumeConfigurators;
    using MassTransit.Definition;
    using Services.Core.Configuration;

    public class EnRouteToCustomerConsumerDefinition :
        ConsumerDefinition<EnRouteToCustomerConsumer>
    {
        readonly RabbitMqTransportSettings _settings;

        public EnRouteToCustomerConsumerDefinition(RabbitMqTransportSettings settings)
        {
            _settings = settings;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EnRouteToCustomerConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseMessageRetry(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRetryImmediatePolicy));
            });
            
            consumerConfigurator.UseScheduledRedelivery(r =>
            {
                r.SetRetryPolicy(x => x.Immediate(_settings.MessageRedeliveryImmediatePolicy));
            });
            
            consumerConfigurator.UseInMemoryOutbox();
        }
    }
}
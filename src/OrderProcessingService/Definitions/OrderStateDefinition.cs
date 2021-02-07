namespace OrderProcessingService.Definitions
{
    using Core.StateMachines.Sagas;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Definition;
    using Services.Core.Configuration;

    public class OrderStateDefinition :
        SagaDefinition<OrderState>
    {
        readonly RabbitMqTransportSettings _settings;

        public OrderStateDefinition(RabbitMqTransportSettings settings)
        {
            _settings = settings;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            sagaConfigurator.UseMessageRetry(r => r.Immediate(_settings.MessageRetryImmediatePolicy));
            sagaConfigurator.UseInMemoryOutbox();
        }
    }
}
namespace OrderProcessingService.Definitions
{
    using Core.StateMachines.Sagas;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Definition;
    using Services.Core.Configuration;

    public class OrderItemStateDefinition :
        SagaDefinition<OrderItemState>
    {
        readonly RabbitMqTransportSettings _settings;

        public OrderItemStateDefinition(RabbitMqTransportSettings settings)
        {
            _settings = settings;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderItemState> sagaConfigurator)
        {
            sagaConfigurator.UseMessageRetry(r => r.Immediate(_settings.MessageRetryImmediatePolicy));
            sagaConfigurator.UseInMemoryOutbox();
        }
    }
}
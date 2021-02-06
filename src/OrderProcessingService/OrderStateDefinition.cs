namespace OrderProcessingService
{
    using Core.StateMachines.Sagas;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Definition;
    using Microsoft.Extensions.Options;
    using Services.Core.Configuration;

    public class OrderStateDefinition :
        SagaDefinition<OrderState>
    {
        readonly RabbitMqTransportSettings _settings;

        public OrderStateDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            sagaConfigurator.UseMessageRetry(r => r.Immediate(_settings.MessageRetryImmediatePolicy));
            sagaConfigurator.UseInMemoryOutbox();
        }
    }
}
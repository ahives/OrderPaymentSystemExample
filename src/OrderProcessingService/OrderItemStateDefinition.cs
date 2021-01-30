namespace OrderProcessingService
{
    using Core.StateMachines.Sagas;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Definition;
    using Microsoft.Extensions.Options;
    using Services.Core.Configuration;

    public class OrderItemStateDefinition :
        SagaDefinition<OrderItemState>
    {
        readonly RabbitMqTransportSettings _settings;

        public OrderItemStateDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderItemState> sagaConfigurator)
        {
            sagaConfigurator.UseMessageRetry(r => r.Immediate(_settings.MessageRetryImmediateCount));
            sagaConfigurator.UseInMemoryOutbox();
        }
    }
}
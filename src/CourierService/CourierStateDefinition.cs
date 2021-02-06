namespace CourierService
{
    using Core.StateMachines.Sagas;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Definition;
    using Microsoft.Extensions.Options;
    using Services.Core.Configuration;

    public class CourierStateDefinition :
        SagaDefinition<CourierState>
    {
        readonly RabbitMqTransportSettings _settings;

        public CourierStateDefinition(IOptions<RabbitMqTransportSettings> options)
        {
            _settings = options.Value;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<CourierState> sagaConfigurator)
        {
            sagaConfigurator.UseMessageRetry(r => r.Immediate(_settings.MessageRetryImmediatePolicy));
            sagaConfigurator.UseInMemoryOutbox();
        }
    }
}
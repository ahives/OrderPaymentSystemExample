namespace CourierService.Definitions
{
    using Core.StateMachines.Sagas;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Definition;
    using Services.Core.Configuration;

    public class CourierStateDefinition :
        SagaDefinition<CourierState>
    {
        readonly RabbitMqTransportSettings _settings;

        public CourierStateDefinition(RabbitMqTransportSettings settings)
        {
            _settings = settings;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<CourierState> sagaConfigurator)
        {
            sagaConfigurator.UseMessageRetry(r => r.Immediate(_settings.MessageRetryImmediatePolicy));
            sagaConfigurator.UseInMemoryOutbox();
        }
    }
}
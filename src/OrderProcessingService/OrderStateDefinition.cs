namespace OrderProcessingService
{
    using Core.StateMachines.Sagas;
    using MassTransit;
    using MassTransit.Definition;

    public class OrderStateDefinition :
        SagaDefinition<OrderState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            sagaConfigurator.UseInMemoryOutbox();
        }
    }
}
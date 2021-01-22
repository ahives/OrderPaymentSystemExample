namespace OrderProcessingService
{
    using Core.StateMachines.Sagas;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Definition;

    public class OrderItemStateDefinition :
        SagaDefinition<OrderItemState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderItemState> sagaConfigurator)
        {
            sagaConfigurator.UseMessageRetry(r => r.Immediate(5));
            sagaConfigurator.UseInMemoryOutbox();
        }
    }
}
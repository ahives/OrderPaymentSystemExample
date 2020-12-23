namespace Restaurant.Core.StateMachines
{
    using Activities;
    using Automatonymous;
    using Sagas;

    public class KitchenStateMachine :
        MassTransitStateMachine<RestaurantState>
    {
        public KitchenStateMachine()
        {
            InstanceState(x => x.CurrentState, Received, PreparingOrder, OrderPrepared, OrderDiscarded);

            Initially(
                When(OrderReceived)
                    .Activity(x => x.OfType<OrderReceivedActivity>())
                    .TransitionTo(Received),
                When(OrderValidated)
                    .Activity(x => x.OfType<BeginOrderPrepActivity>())
                    .TransitionTo(PreparingOrder));
            
            During(Received,
                When(OrderValidated)
                    .Activity(x => x.OfType<BeginOrderPrepActivity>())
                    .TransitionTo(PreparingOrder));
            
            During(PreparingOrder,
                When(OrderValidated)
                    .Activity(x => x.OfType<BeginOrderPrepActivity>())
                    .TransitionTo(OrderPrepared));
            
            Event(() => OrderReceived,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderValidated,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
        }
        
        public State Received { get; }
        public State PreparingOrder { get; }
        public State OrderPrepared { get; }
        public State OrderDiscarded { get; }
        
        public Event<OrderReceived> OrderReceived { get; private set; }
        public Event<OrderValidated> OrderValidated { get; private set; }
    }
}
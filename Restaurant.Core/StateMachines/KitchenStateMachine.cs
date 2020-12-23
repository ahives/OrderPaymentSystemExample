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
            InstanceState(x => x.CurrentState, Received, Preparing, Prepared, Discarded);

            Initially(
                When(OrderReceived)
                    .Activity(x => x.OfType<OrderReceivedActivity>())
                    .TransitionTo(Received),
                When(OrderValidated)
                    .Activity(x => x.OfType<BeginOrderPrepActivity>())
                    .TransitionTo(Preparing));
            
            During(Received,
                When(OrderValidated)
                    .Activity(x => x.OfType<BeginOrderPrepActivity>())
                    .TransitionTo(Preparing));
            
            During(Preparing,
                When(OrderValidated)
                    .Activity(x => x.OfType<BeginOrderPrepActivity>())
                    .TransitionTo(Prepared));

            During(Discarded,
                When(OrderValidated)
                    .Activity(x => x.OfType<BeginOrderPrepActivity>()));
            
            Event(() => OrderReceived,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderValidated,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
        }
        
        public State Received { get; }
        public State Preparing { get; }
        public State Prepared { get; }
        public State Discarded { get; }
        
        public Event<OrderReceived> OrderReceived { get; private set; }
        public Event<OrderValidated> OrderValidated { get; private set; }
    }
}
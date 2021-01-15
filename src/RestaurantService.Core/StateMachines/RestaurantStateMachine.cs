namespace RestaurantService.Core.StateMachines
{
    using Activities;
    using Automatonymous;
    using Sagas;
    using Services.Core.Events;

    public class RestaurantStateMachine :
        MassTransitStateMachine<RestaurantState>
    {
        public RestaurantStateMachine()
        {
            InstanceState(x => x.CurrentState, Received, Preparing, Prepared, Discarded, Canceled, Error);

            Initially(
                When(OrderReceived)
                    .Activity(x => x.OfType<OrderReceiptActivity>())
                    .TransitionTo(Received),
                Ignore(OrderValidated),
                Ignore(OrderNotValidated));
            
            During(Received,
                When(OrderValidated)
                    .Activity(x => x.OfType<BeginOrderPrepActivity>())
                    .TransitionTo(Preparing),
                When(OrderNotValidated)
                    .TransitionTo(Error));
            
            During(Preparing,
                When(OrderPrepared)
                    .Activity(x => x.OfType<OrderPreparedActivity>())
                    .TransitionTo(Prepared),
                When(OrderNotValidated)
                    .TransitionTo(Error),
                When(KitchenMalfunction)
                    .TransitionTo(Error));
            
            During(Prepared,
                When(OrderCanceled)
                    .Activity(x => x.OfType<RestaurantCanceledOrderActivity>())
                    .TransitionTo(Prepared),
                When(OrderDiscarded)
                    .Activity(x => x.OfType<OrderDiscardedActivity>())
                    .TransitionTo(Discarded),
                When(OrderNotValidated)
                    .TransitionTo(Error),
                When(KitchenMalfunction)
                    .TransitionTo(Error));
        }
        
        public State Received { get; }
        public State Preparing { get; }
        public State Prepared { get; }
        public State Discarded { get; }
        public State Canceled { get; }
        public State Error { get; }
        
        public Event<OrderReceived> OrderReceived { get; private set; }
        public Event<OrderValidated> OrderValidated { get; private set; }
        public Event<OrderPrepared> OrderPrepared { get; private set; }
        public Event<OrderDiscarded> OrderDiscarded { get; private set; }
        public Event<OrderCanceled> OrderCanceled { get; private set; }
        public Event<OrderNotValidated> OrderNotValidated { get; private set; }
        public Event<KitchenMalfunction> KitchenMalfunction { get; private set; }
        public Event<StorageCapacityExceeded> StorageCapacityExceeded { get; private set; }
    }
}
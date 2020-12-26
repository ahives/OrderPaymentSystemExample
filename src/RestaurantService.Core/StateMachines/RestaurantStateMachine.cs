namespace RestaurantService.Core.StateMachines
{
    using Activities;
    using Automatonymous;
    using Restaurant.Core;
    using Sagas;

    public class RestaurantStateMachine :
        MassTransitStateMachine<RestaurantState>
    {
        public RestaurantStateMachine()
        {
            InstanceState(x => x.CurrentState, Received, Preparing, Prepared, Discarded, Canceled, Error);

            Initially(
                When(OrderReceived)
                    .Activity(x => x.OfType<OrderReceivedActivity>())
                    .TransitionTo(Received),
                When(OrderValidated)
                    .Activity(x => x.OfType<BeginOrderPrepActivity>())
                    .TransitionTo(Preparing),
                When(OrderNotValidated)
                    .TransitionTo(Error));
            
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
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(Prepared),
                When(OrderDiscarded)
                    .Activity(x => x.OfType<OrderDiscardedActivity>())
                    .TransitionTo(Discarded),
                When(OrderNotValidated)
                    .TransitionTo(Error),
                When(KitchenMalfunction)
                    .TransitionTo(Error));
            
            Event(() => OrderReceived,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderValidated,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderPrepared,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderDiscarded,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderCanceled,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderNotValidated,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => KitchenMalfunction,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => StorageCapacityExceeded,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
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
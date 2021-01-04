namespace CourierService.Core.StateMachines
{
    using Activities;
    using Automatonymous;
    using Sagas;
    using Services.Core.Events;

    public class CourierStateMachine :
        MassTransitStateMachine<CourierState>
    {
        public CourierStateMachine()
        {
            InstanceState(x => x.CurrentState, Dispatched, DispatchConfirmed, EnRouteToRestaurant, OrderPickedUp,
                EnRouteToCustomer, OrderDelivered, OrderCanceled, DispatchDeclined);

            Initially(When(CourierDispatchedEvent)
                    .Activity(x => x.OfType<CourierDispatchActivity>())
                    .TransitionTo(Dispatched),
                Ignore(OrderCanceledEvent));

            During(Dispatched,
                When(CourierDispatchConfirmedEvent)
                    .Activity(x => x.OfType<CourierDispatchConfirmationActivity>())
                    .TransitionTo(DispatchConfirmed),
                When(CourierDispatchDeclinedEvent)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(DispatchDeclined),
                When(OrderExpiredEvent)
                    .Activity(x => x.OfType<OrderExpiredActivity>())
                    .TransitionTo(OrderCanceled),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(OrderCanceled),
                Ignore(CourierDispatchedEvent));

            During(DispatchConfirmed,
                When(CourierEnRouteRestaurantEvent)
                    .Activity(x => x.OfType<CourierEnRouteToRestaurantActivity>())
                    .TransitionTo(EnRouteToRestaurant),
                When(CourierDispatchDeclinedEvent)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(DispatchDeclined),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(OrderCanceled),
                Ignore(CourierDispatchedEvent));

            During(EnRouteToRestaurant,
                When(OrderPickedUpEvent)
                    .Activity(x => x.OfType<CourierPickedUpOrderActivity>())
                    .TransitionTo(OrderPickedUp),
                When(CourierDispatchDeclinedEvent)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(DispatchDeclined));
            
            During(OrderPickedUp,
                When(OrderDeliveredEvent)
                    .Activity(x => x.OfType<OrderDeliveryActivity>())
                    .TransitionTo(OrderDelivered),
                When(CourierDispatchDeclinedEvent)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(DispatchDeclined),
                Ignore(OrderCanceledEvent),
                Ignore(OrderExpiredEvent),
                Ignore(CourierEnRouteRestaurantEvent),
                Ignore(CourierDispatchedEvent));

            During(EnRouteToCustomer,
                When(CourierEnRouteToCustomerEvent)
                    .Activity(x => x.OfType<CourierEnRouteToCustomerActivity>())
                    .TransitionTo(OrderDelivered),
                Ignore(CourierDispatchedEvent),
                Ignore(OrderCanceledEvent));
            
            During(OrderDelivered,
                Ignore(OrderExpiredEvent),
                Ignore(OrderCanceledEvent),
                Ignore(CourierDispatchDeclinedEvent),
                Ignore(CourierDispatchedEvent));
            
            During(OrderCanceled,
                Ignore(OrderCanceledEvent),
                Ignore(CourierDispatchedEvent),
                Ignore(CourierDispatchDeclinedEvent),
                Ignore(OrderPickedUpEvent));
        }
        
        public State Dispatched { get; }
        public State DispatchConfirmed { get; }
        public State OrderPickedUp { get; }
        public State OrderDelivered { get; }
        public State OrderCanceled { get; }
        public State DispatchDeclined { get; }
        public State EnRouteToRestaurant { get; }
        public State EnRouteToCustomer { get; }
        
        public Event<CourierDispatched> CourierDispatchedEvent { get; }
        public Event<OrderPickedUp> OrderPickedUpEvent { get; }
        public Event<OrderDelivered> OrderDeliveredEvent { get; }
        public Event<CourierDispatchConfirmed> CourierDispatchConfirmedEvent { get; }
        public Event<OrderCanceled> OrderCanceledEvent { get; }
        public Event<OrderExpired> OrderExpiredEvent { get; }
        public Event<CourierDispatchDeclined> CourierDispatchDeclinedEvent { get; }
        public Event<CourierEnRouteToRestaurant> CourierEnRouteRestaurantEvent { get; }
        public Event<CourierEnRouteToCustomer> CourierEnRouteToCustomerEvent { get; }
    }
}
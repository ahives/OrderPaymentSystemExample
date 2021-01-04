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
            InstanceState(x => x.CurrentState, Dispatched, Confirmed, EnRouteToRestaurant, PickedUp, EnRouteToCustomer, Delivered, Canceled, Declined);

            Initially(When(CourierDispatched)
                    .Activity(x => x.OfType<CourierDispatchActivity>())
                    .TransitionTo(Dispatched),
                Ignore(OrderCanceled));

            During(Dispatched,
                When(CourierDispatchConfirmed)
                    .Activity(x => x.OfType<CourierDispatchConfirmationActivity>())
                    .TransitionTo(Confirmed),
                When(CourierDeclined)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(Declined),
                When(OrderExpired)
                    .Activity(x => x.OfType<OrderExpiredActivity>())
                    .TransitionTo(Canceled),
                When(OrderCanceled)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(Canceled),
                Ignore(CourierDispatched));

            During(Confirmed,
                When(CourierEnRouteRestaurant)
                    .Activity(x => x.OfType<CourierEnRouteToRestaurantActivity>())
                    .TransitionTo(EnRouteToRestaurant),
                When(CourierDeclined)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(Declined),
                When(OrderCanceled)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(Canceled),
                Ignore(CourierDispatched));

            During(EnRouteToRestaurant,
                When(OrderPickedUp)
                    .Activity(x => x.OfType<CourierPickedUpOrderActivity>())
                    .TransitionTo(PickedUp),
                When(CourierDeclined)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(Declined));
            
            During(PickedUp,
                When(OrderDelivered)
                    .Activity(x => x.OfType<OrderDeliveryActivity>())
                    .TransitionTo(Delivered),
                When(CourierDeclined)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(Declined),
                Ignore(OrderCanceled),
                Ignore(OrderExpired),
                Ignore(CourierEnRouteRestaurant),
                Ignore(CourierDispatched));

            During(EnRouteToCustomer,
                When(CourierEnRouteToCustomer)
                    .Activity(x => x.OfType<CourierEnRouteToCustomerActivity>())
                    .TransitionTo(Delivered));
            
            During(Delivered,
                Ignore(OrderExpired),
                Ignore(OrderCanceled),
                Ignore(CourierDeclined),
                Ignore(CourierDispatched));
            
            During(Canceled,
                Ignore(OrderCanceled),
                Ignore(CourierDispatched),
                Ignore(CourierDeclined),
                Ignore(OrderPickedUp));
        }
        
        public State Dispatched { get; }
        public State Confirmed { get; }
        public State PickedUp { get; }
        public State Delivered { get; }
        public State Canceled { get; }
        public State Declined { get; }
        public State EnRouteToRestaurant { get; }
        public State EnRouteToCustomer { get; }
        
        public Event<CourierDispatched> CourierDispatched { get; }
        public Event<OrderPickedUp> OrderPickedUp { get; }
        public Event<OrderDelivered> OrderDelivered { get; }
        public Event<CourierDispatchConfirmed> CourierDispatchConfirmed { get; }
        public Event<OrderCanceled> OrderCanceled { get; }
        public Event<OrderExpired> OrderExpired { get; }
        public Event<CourierDeclined> CourierDeclined { get; }
        public Event<CourierEnRouteToRestaurant> CourierEnRouteRestaurant { get; }
        public Event<CourierEnRouteToCustomer> CourierEnRouteToCustomer { get; }
    }
}
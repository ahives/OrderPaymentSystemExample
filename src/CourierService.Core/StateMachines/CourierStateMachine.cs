namespace CourierService.Core.StateMachines
{
    using System;
    using Activities;
    using Automatonymous;
    using MassTransit;
    using Sagas;
    using Services.Core.Events;

    public class CourierStateMachine :
        MassTransitStateMachine<CourierState>
    {
        public CourierStateMachine()
        {
            Schedule(() => OrderCompletionTimeout, instance => instance.OrderCompletionTimeoutTokenId, s =>
            {
                s.Delay = TimeSpan.FromMinutes(1);
                s.Received = r => r.CorrelateById(context => context.Message.OrderId);
            });

            InstanceState(x => x.CurrentState, Dispatched, DispatchConfirmed, EnRouteToRestaurant, ArrivedAtRestaurant,
                EnRouteToCustomer, DeliveringOrder, OrderDelivered, DispatchCanceled, DispatchDeclined);

            Initially(When(CourierDispatchedEvent)
                    .Activity(x => x.OfType<CourierDispatchActivity>())
                    .TransitionTo(Dispatched),
                Ignore(OrderCanceledEvent));

            During(Dispatched,
                When(OrderReadyForDeliveryEvent)
                    .Activity(x => x.OfType<OrderReadyForDeliveryActivity>())
                    .TransitionTo(Dispatched),
                When(CourierDispatchConfirmedEvent)
                    .Activity(x => x.OfType<CourierDispatchConfirmationActivity>())
                    .TransitionTo(DispatchConfirmed),
                When(CourierDispatchDeclinedEvent)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(DispatchDeclined),
                When(OrderExpiredEvent)
                    .Activity(x => x.OfType<OrderExpiredActivity>())
                    .TransitionTo(DispatchCanceled),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(DispatchCanceled),
                Ignore(CourierDispatchedEvent));

            During(DispatchConfirmed,
                When(OrderReadyForDeliveryEvent)
                    .Activity(x => x.OfType<OrderReadyForDeliveryActivity>())
                    .TransitionTo(DispatchConfirmed),
                When(CourierEnRouteRestaurantEvent)
                    .Activity(x => x.OfType<CourierEnRouteToRestaurantActivity>())
                    .TransitionTo(EnRouteToRestaurant),
                When(CourierDispatchDeclinedEvent)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(DispatchDeclined),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(DispatchCanceled),
                Ignore(CourierDispatchedEvent));

            During(EnRouteToRestaurant,
                When(CourierArrivedAtRestaurantEvent)
                    .If(context => !context.Instance.IsOrderReady,
                        binder =>
                            binder.Schedule(OrderCompletionTimeout, context =>
                                context.Init<OrderCompletionTimeoutExpired>(new
                                {
                                    context.Instance.OrderId,
                                    context.Instance.RestaurantId,
                                    context.Instance.CourierId,
                                    context.Instance.CustomerId
                                })))
                    .Activity(x => x.OfType<CourierArrivedAtRestaurantActivity>())
                    .TransitionTo(ArrivedAtRestaurant),
                When(OrderReadyForDeliveryEvent)
                    .Activity(x => x.OfType<OrderReadyForDeliveryActivity>())
                    .Unschedule(OrderCompletionTimeout)
                    .TransitionTo(EnRouteToRestaurant),
                When(OrderCompletionTimeout.Received)
                    .Activity(x => x.OfType<OrderCompletionTimeoutActivity>())
                    .TransitionTo(DispatchDeclined),
                When(CourierDispatchDeclinedEvent)
                    .Activity(x => x.OfType<CourierDeclinedActivity>())
                    .TransitionTo(DispatchDeclined),
                When(OrderExpiredEvent)
                    .Activity(x => x.OfType<OrderExpiredActivity>())
                    .TransitionTo(DispatchCanceled),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(DispatchCanceled));

            During(ArrivedAtRestaurant,
                When(OrderReadyForDeliveryEvent)
                    .Activity(x => x.OfType<OrderReadyForPickUpActivity>())
                    .Unschedule(OrderCompletionTimeout)
                    .TransitionTo(ArrivedAtRestaurant),
                When(OrderPickedUpEvent)
                    .Activity(x => x.OfType<CourierPickedUpOrderActivity>())
                    .Unschedule(OrderCompletionTimeout)
                    .TransitionTo(EnRouteToCustomer),
                When(OrderCompletionTimeout.Received)
                    .Activity(x => x.OfType<OrderCompletionTimeoutActivity>())
                    .TransitionTo(DispatchDeclined),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .Unschedule(OrderCompletionTimeout)
                    .TransitionTo(DispatchCanceled),
                When(OrderExpiredEvent)
                    .Activity(x => x.OfType<OrderExpiredActivity>())
                    .Unschedule(OrderCompletionTimeout)
                    .TransitionTo(DispatchCanceled),
                Ignore(CourierEnRouteRestaurantEvent),
                Ignore(CourierDispatchedEvent));

            During(EnRouteToCustomer,
                When(CourierEnRouteToCustomerEvent)
                    .Activity(x => x.OfType<CourierEnRouteToCustomerActivity>())
                    .TransitionTo(DeliveringOrder),
                Ignore(CourierDispatchedEvent),
                Ignore(OrderCanceledEvent));
            
            During(DeliveringOrder,
                When(DeliveringOrderEvent)
                    .Activity(x => x.OfType<DeliveringOrderActivity>())
                    .TransitionTo(OrderDelivered));
            
            During(OrderDelivered,
                Ignore(OrderExpiredEvent),
                Ignore(OrderCanceledEvent),
                Ignore(CourierDispatchDeclinedEvent),
                Ignore(CourierDispatchedEvent));
            
            During(DispatchCanceled,
                Ignore(DeliveringOrderEvent),
                Ignore(CourierEnRouteToCustomerEvent),
                Ignore(CourierArrivedAtRestaurantEvent),
                Ignore(OrderCanceledEvent),
                Ignore(OrderReadyForDeliveryEvent),
                Ignore(CourierDispatchedEvent),
                Ignore(CourierDispatchDeclinedEvent),
                Ignore(OrderPickedUpEvent));
        }
        
        public State Dispatched { get; }
        public State DispatchConfirmed { get; }
        public State OrderDelivered { get; }
        public State DispatchCanceled { get; }
        public State DispatchDeclined { get; }
        public State EnRouteToRestaurant { get; }
        public State EnRouteToCustomer { get; }
        public State DeliveringOrder { get; }
        public State ArrivedAtRestaurant { get; }
        
        public Event<CourierDispatched> CourierDispatchedEvent { get; }
        public Event<OrderPickedUp> OrderPickedUpEvent { get; }
        public Event<CourierDispatchConfirmed> CourierDispatchConfirmedEvent { get; }
        public Event<OrderCanceled> OrderCanceledEvent { get; }
        public Event<OrderExpired> OrderExpiredEvent { get; }
        public Event<CourierDispatchDeclined> CourierDispatchDeclinedEvent { get; }
        public Event<CourierEnRouteToRestaurant> CourierEnRouteRestaurantEvent { get; }
        public Event<CourierEnRouteToCustomer> CourierEnRouteToCustomerEvent { get; }
        public Event<OrderReadyForDelivery> OrderReadyForDeliveryEvent { get; }
        public Event<CourierArrivedAtRestaurant> CourierArrivedAtRestaurantEvent { get; }
        public Event<DeliveringOrder> DeliveringOrderEvent { get; }
        
        public Schedule<CourierState, OrderCompletionTimeoutExpired> OrderCompletionTimeout { get; }
    }
}
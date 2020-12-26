namespace CourierService.StateMachines
{
    using Activities;
    using Automatonymous;
    using Restaurant.Core;
    using Sagas;

    public class CourierStateMachine :
        MassTransitStateMachine<CourierState>
    {
        public CourierStateMachine()
        {
            InstanceState(x => x.CurrentState, Dispatched, ConfirmedDispatch, OrderPickedUp, Delivered, Recalled);

            Initially(When(CourierDispatched)
                    .TransitionTo(Dispatched),
                Ignore(OrderCanceled));

            During(Dispatched,
                When(CourierConfirmed)
                    .TransitionTo(ConfirmedDispatch),
                When(OrderExpired)
                    .Activity(x => x.OfType<OrderExpiredActivity>())
                    .TransitionTo(Recalled),
                When(OrderCanceled)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(Recalled),
                Ignore(CourierDispatched));

            During(ConfirmedDispatch,
                When(OrderPickedUpByCourier)
                    .TransitionTo(OrderPickedUp),
                When(OrderCanceled)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(Recalled),
                Ignore(CourierDispatched));

            During(OrderPickedUp,
                When(CourierDeliveredOrder)
                    .TransitionTo(Delivered),
                Ignore(OrderCanceled),
                Ignore(OrderExpired),
                Ignore(CourierDispatched));

            During(Delivered,
                Ignore(OrderExpired),
                Ignore(OrderCanceled),
                Ignore(CourierDispatched));
            
            Event(() => CourierDispatched,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderPickedUpByCourier,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => CourierDeliveredOrder,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => CourierConfirmed,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderCanceled,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderExpired,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
        }
        
        public State Dispatched { get; }
        public State ConfirmedDispatch { get; }
        public State OrderPickedUp { get; }
        public State Delivered { get; }
        public State Recalled { get; }
        
        public Event<CourierDispatched> CourierDispatched { get; }
        public Event<OrderPickedUpByCourier> OrderPickedUpByCourier { get; }
        public Event<CourierDeliveredOrder> CourierDeliveredOrder { get; }
        public Event<CourierConfirmed> CourierConfirmed { get; }
        public Event<OrderCanceled> OrderCanceled { get; }
        public Event<OrderExpired> OrderExpired { get; }
    }
}
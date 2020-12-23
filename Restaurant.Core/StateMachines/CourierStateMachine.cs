namespace Restaurant.Core.StateMachines
{
    using Automatonymous;
    using Sagas;

    public class CourierStateMachine :
        MassTransitStateMachine<CourierState>
    {
        public CourierStateMachine()
        {
            InstanceState(x => x.CurrentState, Dispatched, Confirmed, PickedUp, Delivered);
            
            Initially(When(CourierDispatched)
                .TransitionTo(Dispatched));
            
            During(Dispatched,
                When(CourierConfirmed)
                .TransitionTo(Confirmed));
            
            During(Confirmed,
                When(OrderPickedUpByCourier)
                .TransitionTo(PickedUp));
            
            During(PickedUp,
                When(CourierDeliveredOrder)
                .TransitionTo(Delivered));
            
            Event(() => CourierDispatched,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderPickedUpByCourier,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => CourierDeliveredOrder,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => CourierConfirmed,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
        }
        
        public State Dispatched { get; }
        public State Confirmed { get; }
        public State PickedUp { get; }
        public State Delivered { get; }
        
        public Event<CourierDispatched> CourierDispatched { get; }
        public Event<OrderPickedUpByCourier> OrderPickedUpByCourier { get; }
        public Event<CourierDeliveredOrder> CourierDeliveredOrder { get; }
        public Event<CourierConfirmed> CourierConfirmed { get; }
    }
}
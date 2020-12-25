namespace RestaurantService.StateMachines
{
    using Automatonymous;
    using Restaurant.Core;
    using Sagas;

    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState, Received, Pending);

            // Initially(
            //     When(OrderReceived)
            //         .Activity(x => x.OfType<OrderReceivedActivity>())
            //         .TransitionTo(Received),
            //     When(OrderValidated)
            //         .Activity(x => x.OfType<BeginOrderPrepActivity>())
            //         .TransitionTo(Pending));
            //
            // During(Received,
            //     When(OrderValidated)
            //         .Activity(x => x.OfType<BeginOrderPrepActivity>())
            //         .TransitionTo(Pending));
            
            Event(() => OrderReceived,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderValidated,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
        }

        public State Received { get; }
        public State Validated { get; }
        public State Pending { get; }

        public Event<OrderReceived> OrderReceived { get; private set; }
        public Event<OrderValidated> OrderValidated { get; private set; }
    }
}
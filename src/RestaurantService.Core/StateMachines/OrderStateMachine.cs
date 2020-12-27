namespace RestaurantService.Core.StateMachines
{
    using Activities;
    using Automatonymous;
    using Sagas;
    using Services.Core.Events;

    public class OrderStateMachine :
        MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState, Pending);

            Initially(
                When(PrepareOrder)
                    .Activity(x => x.OfType<PrepareOrderRequestedActivity>())
                    .TransitionTo(Pending));
            
            During(Pending,
                When(OrderItemPrepared)
                    // .Activity(x => x.OfType<PrepareOrderRequestedActivity>())
                    .TransitionTo(Prepared));
            
            Event(() => PrepareOrder,
                x => x.CorrelateById(cxt => cxt.Message.OrderId));
        }

        public State Pending { get; }
        public State Prepared { get; }

        public Event<PrepareOrder> PrepareOrder { get; private set; }
        public Event<OrderItemPrepared> OrderItemPrepared { get; private set; }
    }
}
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
            InstanceState(x => x.CurrentState, Pending, Prepared, NotPrepared, Canceled);

            Initially(
                When(PrepareOrder)
                    .Activity(x => x.OfType<PrepareOrderRequestedActivity>())
                    .TransitionTo(Pending));

            During(Pending,
                When(OrderItemPrepared)
                    .Activity(x => x.OfType<OrderItemsBeingPreparedActivity>())
                    .IfElse(context => context.Instance.ItemCount == context.Instance.Items.Count,
                        thenBinder => thenBinder.TransitionTo(Prepared),
                        elseBinder => elseBinder.TransitionTo(Pending)),
                When(OrderItemNotPrepared)
                    .TransitionTo(NotPrepared));

            During(Canceled,
                When(OrderCanceled)
                    .TransitionTo(Canceled));
        }

        public State Pending { get; }
        public State Prepared { get; }
        public State Canceled { get; }
        public State NotPrepared { get; }

        public Event<PrepareOrder> PrepareOrder { get; private set; }
        public Event<OrderItemPrepared> OrderItemPrepared { get; private set; }
        public Event<OrderCanceled> OrderCanceled { get; private set; }
        public Event<OrderItemNotPrepared> OrderItemNotPrepared { get; private set; }
    }
}
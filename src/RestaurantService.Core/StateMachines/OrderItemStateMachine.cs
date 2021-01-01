namespace RestaurantService.Core.StateMachines
{
    using Activities;
    using Automatonymous;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemStateMachine :
        MassTransitStateMachine<OrderItemState>
    {
        public OrderItemStateMachine()
        {
            InstanceState(x => x.CurrentState, Preparing, Prepared, Discarded, Canceled, Expired, NotPrepared);

            Initially(When(PrepareOrderItemRequested)
                .Activity(x => x.OfType<PrepareOrderItemRequestActivity>())
                .TransitionTo(Preparing));

            During(Preparing,
                When(OrderItemPrepared)
                    .Activity(x => x.OfType<OrderItemPreparedActivity>())
                    .TransitionTo(Prepared),
                When(OrderItemNotPrepared)
                    .Activity(x => x.OfType<OrderItemNotPreparedActivity>())
                    .TransitionTo(NotPrepared),
                When(OrderCanceled)
                    .Activity(x => x.OfType<CancelOrderItemActivity>())
                    .TransitionTo(Canceled),
                Ignore(PrepareOrderItemRequested));
            
            During(Prepared,
                When(OrderItemDiscarded)
                    .Activity(x => x.OfType<DiscardOrderItemActivity>())
                    .TransitionTo(Discarded),
                When(OrderItemExpired)
                    .Activity(x => x.OfType<ExpireOrderItemActivity>())
                    .TransitionTo(Expired),
                When(OrderCanceled)
                    .Activity(x => x.OfType<CancelOrderItemActivity>())
                    .TransitionTo(Canceled));

            During(Expired,
                When(OrderItemExceededPreparationLimit)
                    .Activity(x => x.OfType<OrderItemExpiredActivity>())
                    .TransitionTo(NotPrepared),
                Ignore(OrderItemPrepared),
                Ignore(PrepareOrderItemRequested));
            
            During(Discarded,
                When(OrderItemExceededPreparationLimit)
                    .Activity(x => x.OfType<OrderItemDiscardedActivity>())
                    .TransitionTo(NotPrepared),
                Ignore(OrderItemPrepared),
                Ignore(PrepareOrderItemRequested),
                Ignore(OrderItemExpired));

            During(NotPrepared,
                Ignore(OrderCanceled),
                Ignore(OrderItemCanceled),
                Ignore(OrderItemExpired),
                Ignore(OrderItemDiscarded));
        }
        
        public State Preparing { get; }
        public State Prepared { get; }
        public State Discarded { get; }
        public State Canceled { get; }
        public State Expired { get; }
        public State NotPrepared { get; }
        
        public Event<PrepareOrderItemRequested> PrepareOrderItemRequested { get; private set; }
        public Event<OrderItemPrepared> OrderItemPrepared { get; private set; }
        public Event<OrderItemExpired> OrderItemExpired { get; private set; }
        public Event<OrderItemDiscarded> OrderItemDiscarded { get; private set; }
        public Event<OrderCanceled> OrderCanceled { get; private set; }
        public Event<OrderItemCanceled> OrderItemCanceled { get; private set; }
        public Event<OrderItemExceededPreparationLimit> OrderItemExceededPreparationLimit { get; private set; }
        public Event<OrderItemNotPrepared> OrderItemNotPrepared { get; private set; }
    }
}
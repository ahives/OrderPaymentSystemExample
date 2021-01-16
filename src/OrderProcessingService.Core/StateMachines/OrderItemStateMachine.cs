namespace OrderProcessingService.Core.StateMachines
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

            Initially(When(PrepareOrderItemRequestedEvent)
                .Activity(x => x.OfType<PrepareOrderItemRequestActivity>())
                .TransitionTo(Preparing));

            During(Preparing,
                When(OrderItemPreparedEvent)
                    .Activity(x => x.OfType<OrderItemPreparedActivity>())
                    .TransitionTo(Prepared),
                When(OrderItemNotPreparedEvent)
                    .Activity(x => x.OfType<OrderItemNotPreparedActivity>())
                    .TransitionTo(NotPrepared),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<CancelOrderItemActivity>())
                    .TransitionTo(Canceled),
                Ignore(PrepareOrderItemRequestedEvent));
            
            During(Prepared,
                When(OrderItemDiscardedEvent)
                    .Activity(x => x.OfType<DiscardOrderItemActivity>())
                    .TransitionTo(Discarded),
                When(OrderItemExpiredEvent)
                    .Activity(x => x.OfType<ExpireOrderItemActivity>())
                    .TransitionTo(Expired),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<CancelOrderItemActivity>())
                    .TransitionTo(Canceled));

            During(Expired,
                When(OrderItemExceededPreparationLimitEvent)
                    .Activity(x => x.OfType<OrderItemExpiredActivity>())
                    .TransitionTo(NotPrepared),
                Ignore(OrderItemPreparedEvent),
                Ignore(PrepareOrderItemRequestedEvent));
            
            During(Discarded,
                When(OrderItemExceededPreparationLimitEvent)
                    .Activity(x => x.OfType<OrderItemDiscardedActivity>())
                    .TransitionTo(NotPrepared),
                Ignore(OrderItemPreparedEvent),
                Ignore(PrepareOrderItemRequestedEvent),
                Ignore(OrderItemExpiredEvent));

            During(NotPrepared,
                Ignore(OrderCanceledEvent),
                Ignore(OrderItemCanceledEvent),
                Ignore(OrderItemExpiredEvent),
                Ignore(OrderItemDiscardedEvent));
        }
        
        public State Preparing { get; }
        public State Prepared { get; }
        public State Discarded { get; }
        public State Canceled { get; }
        public State Expired { get; }
        public State NotPrepared { get; }
        
        public Event<PrepareOrderItemRequested> PrepareOrderItemRequestedEvent { get; private set; }
        public Event<OrderItemPrepared> OrderItemPreparedEvent { get; private set; }
        public Event<OrderItemExpired> OrderItemExpiredEvent { get; private set; }
        public Event<OrderItemDiscarded> OrderItemDiscardedEvent { get; private set; }
        public Event<OrderCanceled> OrderCanceledEvent { get; private set; }
        public Event<OrderItemCanceled> OrderItemCanceledEvent { get; private set; }
        public Event<OrderItemExceededPreparationLimit> OrderItemExceededPreparationLimitEvent { get; private set; }
        public Event<OrderItemNotPrepared> OrderItemNotPreparedEvent { get; private set; }
    }
}
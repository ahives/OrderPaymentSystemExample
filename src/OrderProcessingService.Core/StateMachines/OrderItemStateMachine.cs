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
            Event(() => RequestOrderItemPreparationEvent, e => e.CorrelateById(context => context.Message.OrderItemId));
            Event(() => OrderItemPreparedEvent, e => e.CorrelateById(context => context.Message.OrderItemId));
            Event(() => OrderItemNotPreparedEvent, e => e.CorrelateById(context => context.Message.OrderItemId));
            Event(() => OrderItemDiscardedEvent, e => e.CorrelateById(context => context.Message.OrderItemId));
            // Event(() => OrderCanceledEvent, e => e.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderItemCancelRequestEvent, e => e.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderItemCanceledEvent, e => e.CorrelateById(context => context.Message.OrderItemId));

            InstanceState(x => x.CurrentState, Preparing, Prepared, Discarded, Canceled, Expired, NotPrepared);

            Initially(When(RequestOrderItemPreparationEvent)
                .Activity(x => x.OfType<RequestOrderItemPreparationActivity>())
                .TransitionTo(Preparing));

            During(Preparing,
                When(OrderItemPreparedEvent)
                    .Activity(x => x.OfType<OrderItemPreparedActivity>())
                    .TransitionTo(Prepared),
                When(OrderItemNotPreparedEvent)
                    .Activity(x => x.OfType<OrderItemNotPreparedActivity>())
                    .TransitionTo(NotPrepared),
                When(OrderItemCancelRequestEvent)
                    .Activity(x => x.OfType<OrderItemCancelRequestActivity>())
                    .TransitionTo(Preparing),
                When(OrderItemCanceledEvent)
                    .Activity(x => x.OfType<OrderItemCanceledActivity>())
                    .TransitionTo(Canceled),
                Ignore(RequestOrderItemPreparationEvent));

            During(Prepared,
                When(OrderItemDiscardedEvent)
                    .Activity(x => x.OfType<OrderItemDiscardedActivity>())
                    .TransitionTo(Discarded),
                // When(OrderItemExpiredEvent)
                //     .Activity(x => x.OfType<ExpireOrderItemActivity>())
                //     .TransitionTo(Expired),
                When(OrderItemCancelRequestEvent)
                    .Activity(x => x.OfType<OrderItemCancelRequestActivity>())
                    .TransitionTo(Preparing),
                When(OrderItemCanceledEvent)
                    .Activity(x => x.OfType<OrderItemCanceledActivity>())
                    .TransitionTo(Canceled));
            
            // During(Expired,
            //     When(OrderItemExceededPreparationLimitEvent)
            //         .Activity(x => x.OfType<OrderItemExpiredActivity>())
            //         .TransitionTo(NotPrepared),
            //     Ignore(OrderItemPreparedEvent),
            //     Ignore(RequestOrderItemPreparationEvent));
            //
            // During(Discarded,
            //     When(OrderItemExceededPreparationLimitEvent)
            //         .Activity(x => x.OfType<OrderItemExceededPreparationLimitActivity>())
            //         .TransitionTo(NotPrepared),
            //     Ignore(OrderItemPreparedEvent),
            //     Ignore(RequestOrderItemPreparationEvent),
            //     Ignore(OrderItemExpiredEvent));
            //
            // During(NotPrepared,
            //     Ignore(OrderCanceledEvent),
            //     Ignore(OrderItemCanceledEvent),
            //     Ignore(OrderItemExpiredEvent),
            //     Ignore(OrderItemDiscardedEvent));
            
            // DuringAny(When(OrderCanceledEvent)
            //     .Activity(x => x.OfType<CancelOrderItemActivity>())
            //     .TransitionTo(Canceled));
            //
            // DuringAny(When(OrderItemCancelRequestEvent)
            //     .Activity(x => x.OfType<OrderItemCancelRequestActivity>())
            //     .TransitionTo(Canceled));
        }
        
        public State Preparing { get; }
        public State Prepared { get; }
        public State Discarded { get; }
        public State Canceled { get; }
        public State Expired { get; }
        public State NotPrepared { get; }
        
        public Event<RequestOrderItemPreparation> RequestOrderItemPreparationEvent { get; private set; }
        public Event<OrderItemPrepared> OrderItemPreparedEvent { get; private set; }
        public Event<OrderItemNotPrepared> OrderItemNotPreparedEvent { get; private set; }
        // public Event<OrderItemExpired> OrderItemExpiredEvent { get; private set; }
        public Event<OrderItemDiscarded> OrderItemDiscardedEvent { get; private set; }
        // public Event<OrderCanceled> OrderCanceledEvent { get; private set; }
        public Event<OrderItemCancelRequest> OrderItemCancelRequestEvent { get; private set; }
        public Event<OrderItemCanceled> OrderItemCanceledEvent { get; private set; }
        // public Event<OrderItemExceededPreparationLimit> OrderItemExceededPreparationLimitEvent { get; private set; }
    }
}
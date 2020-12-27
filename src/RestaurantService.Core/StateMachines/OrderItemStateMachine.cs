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

            Initially(When(PrepareOrderItem)
                    .Activity(x => x.OfType<PrepareOrderItemActivity>())
                .TransitionTo(Preparing));

            During(Preparing,
                When(OrderItemPrepared)
                    .Activity(x => x.OfType<OrderItemPreparedActivity>())
                    .TransitionTo(Prepared),
                When(OrderCanceled)
                    .Activity(x => x.OfType<CancelOrderItemActivity>())
                    .TransitionTo(Canceled),
                Ignore(PrepareOrderItem));
            
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
                Ignore(PrepareOrderItem));
            
            During(Discarded,
                When(OrderItemExceededPreparationLimit)
                    .Activity(x => x.OfType<OrderItemDiscardedActivity>())
                    .TransitionTo(NotPrepared),
                Ignore(OrderItemPrepared),
                Ignore(PrepareOrderItem),
                Ignore(OrderItemExpired));

            During(NotPrepared,
                Ignore(OrderCanceled),
                Ignore(OrderItemCanceled),
                Ignore(OrderItemExpired),
                Ignore(OrderItemDiscarded));
            
            Event(() => PrepareOrderItem, x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderItemPrepared, x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderItemExpired, x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderItemDiscarded, x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderCanceled, x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderItemCanceled, x => x.CorrelateById(cxt => cxt.Message.OrderId));
            Event(() => OrderItemExceededPreparationLimit, x => x.CorrelateById(cxt => cxt.Message.OrderId));
        }
        
        public State Preparing { get; }
        public State Prepared { get; }
        public State Discarded { get; }
        public State Canceled { get; }
        public State Expired { get; }
        public State NotPrepared { get; }
        
        public Event<PrepareOrderItem> PrepareOrderItem { get; private set; }
        public Event<OrderItemPrepared> OrderItemPrepared { get; private set; }
        public Event<OrderItemExpired> OrderItemExpired { get; private set; }
        public Event<OrderItemDiscarded> OrderItemDiscarded { get; private set; }
        public Event<OrderCanceled> OrderCanceled { get; private set; }
        public Event<OrderItemCanceled> OrderItemCanceled { get; private set; }
        public Event<OrderItemExceededPreparationLimit> OrderItemExceededPreparationLimit { get; private set; }
    }
}
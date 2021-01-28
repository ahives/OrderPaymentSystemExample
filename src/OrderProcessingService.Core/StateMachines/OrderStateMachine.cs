namespace OrderProcessingService.Core.StateMachines
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
            Event(() => PrepareOrderRequestEvent, e => e.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderItemPreparedEvent, e => e.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderItemExpiredEvent, e => e.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderCancelRequestEvent, e => e.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderItemCanceledEvent, e => e.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderCanceledEvent, e => e.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderItemVoidedEvent, e => e.CorrelateById(context => context.Message.OrderId));
            
            InstanceState(x => x.CurrentState, Pending, Prepared, NotPrepared, Canceled);

            Initially(When(PrepareOrderRequestEvent)
                    .Activity(x => x.OfType<PrepareOrderRequestedActivity>())
                    .TransitionTo(Pending));

            During(Pending,
                When(OrderItemPreparedEvent)
                    .Activity(x => x.OfType<OrderItemsPreparedActivity>())
                    .IfElse(context => context.Instance.PreparedItemCount == context.Instance.ExpectedItemCount,
                        thenBinder => thenBinder.TransitionTo(Prepared),
                        elseBinder => elseBinder.TransitionTo(Pending)),
                When(OrderItemExpiredEvent)
                    .Activity(x => x.OfType<OrderItemsExpiredActivity>())
                    .TransitionTo(Pending),
                When(OrderCancelRequestEvent)
                    .Activity(x => x.OfType<OrderCancelRequestActivity>())
                    .TransitionTo(Pending),
                When(OrderItemCanceledEvent)
                    .Activity(x => x.OfType<OrderItemsCanceledActivity>())
                    .IfElse(context => context.Instance.CanceledItemCount == context.Instance.ExpectedItemCount,
                        thenBinder => thenBinder.TransitionTo(Canceled),
                        elseBinder => elseBinder.TransitionTo(Pending)),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(Canceled));

            During(Prepared,
                When(OrderCancelRequestEvent)
                    .Activity(x => x.OfType<OrderCancelRequestActivity>())
                    .TransitionTo(Prepared),
                When(OrderItemCanceledEvent)
                    .Activity(x => x.OfType<OrderItemsCanceledActivity>())
                    .IfElse(context => context.Instance.CanceledItemCount == context.Instance.ExpectedItemCount,
                        thenBinder => thenBinder.TransitionTo(Canceled),
                        elseBinder => elseBinder.TransitionTo(Prepared)),
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(Canceled),
                When(OrderItemExpiredEvent)
                    .Activity(x => x.OfType<OrderItemsExpiredActivity>())
                    .TransitionTo(Pending));
            
            During(Canceled,
                When(OrderCanceledEvent)
                    .Activity(x => x.OfType<OrderCanceledActivity>())
                    .TransitionTo(Canceled));
            
            DuringAny(
                When(OrderItemVoidedEvent)
                .Activity(x => x.OfType<OrderItemsVoidedActivity>()));
        }

        public State Pending { get; }
        public State Prepared { get; }
        public State Canceled { get; }
        public State NotPrepared { get; }

        public Event<RequestOrderPreparation> PrepareOrderRequestEvent { get; private set; }
        public Event<OrderItemPrepared> OrderItemPreparedEvent { get; private set; }
        public Event<OrderCanceled> OrderCanceledEvent { get; private set; }
        public Event<OrderCancelRequest> OrderCancelRequestEvent { get; private set; }
        public Event<OrderItemExpired> OrderItemExpiredEvent { get; private set; }
        public Event<OrderItemCanceled> OrderItemCanceledEvent { get; private set; }
        public Event<OrderItemVoided> OrderItemVoidedEvent { get; private set; }
    }
}
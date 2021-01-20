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
            Event(() => OrderItemPreparedEvent, e => e.CorrelateById(context => context.Message.OrderItemId));
            Event(() => OrderItemNotPreparedEvent, e => e.CorrelateById(context => context.Message.OrderItemId));
            
            InstanceState(x => x.CurrentState, Pending, Prepared, NotPrepared, Canceled);

            Initially(When(PrepareOrderRequestEvent)
                    .Activity(x => x.OfType<PrepareOrderRequestedActivity>())
                    .TransitionTo(Pending));

            During(Pending,
                When(OrderItemPreparedEvent)
                    .Activity(x => x.OfType<OrderItemsBeingPreparedActivity>())
                    .IfElse(context => context.Instance.ActualItemCount == context.Instance.ExpectedItemCount,
                        thenBinder => thenBinder.TransitionTo(Prepared),
                        elseBinder => elseBinder.TransitionTo(Pending)),
                When(OrderItemNotPreparedEvent)
                    .TransitionTo(NotPrepared));

            // During(Canceled,
            //     When(OrderCanceled)
            //         .Activity(x => x.OfType<OrderCanceledActivity>())
            //         .TransitionTo(Canceled));
        }

        public State Pending { get; }
        public State Prepared { get; }
        public State Canceled { get; }
        public State NotPrepared { get; }

        public Event<RequestOrderPreparation> PrepareOrderRequestEvent { get; private set; }
        public Event<OrderItemPrepared> OrderItemPreparedEvent { get; private set; }
        // public Event<OrderCanceled> OrderCanceled { get; private set; }
        public Event<OrderItemNotPrepared> OrderItemNotPreparedEvent { get; private set; }
        // public Event<PrepareOrder> PrepareOrder { get; private set; }
    }
}
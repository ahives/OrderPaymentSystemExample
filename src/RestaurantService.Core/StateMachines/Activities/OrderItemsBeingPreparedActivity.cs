namespace RestaurantService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemsBeingPreparedActivity :
        Activity<OrderState, OrderItemPrepared>
    {
        public OrderItemsBeingPreparedActivity()
        {
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderItemPrepared> context,
            Behavior<OrderState, OrderItemPrepared> next)
        {
            ExpectedOrderItem orderItem = null;

            int i = 0;
            while (i < context.Instance.Items.Count)
            {
                if (context.Instance.Items[i].CorrelationId == context.Data.OrderItemId)
                {
                    context.Instance.Items[i].Status = context.Data.Status;
                    context.Instance.Items[i].Timestamp = context.Data.Timestamp;
                
                    orderItem = context.Instance.Items[i];
                    break;
                }

                i++;
            }
            
            if (orderItem != null)
            {
                if (orderItem.Status == (int)OrderItemStatus.Prepared)
                    context.Instance.ItemCount += 1;
                else
                    context.Instance.ItemCount -= 1;
            }
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<OrderState, OrderItemPrepared, TException> context,
            Behavior<OrderState, OrderItemPrepared> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
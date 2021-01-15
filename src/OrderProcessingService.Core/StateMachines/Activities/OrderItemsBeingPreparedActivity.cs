namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemsBeingPreparedActivity :
        Activity<OrderState, OrderItemPrepared>
    {
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
            for (int i = 0; i < context.Instance.Items.Count; i++)
            {
                if (context.Instance.Items[i].CorrelationId != context.Data.OrderItemId)
                    continue;
                
                context.Instance.Items[i].Status = context.Data.Status;
                context.Instance.Items[i].Timestamp = context.Data.Timestamp;
                break;
            }

            context.Instance.ActualItemCount = context.Instance.Items
                .Count(x => x.Status == (int) OrderItemStatus.Prepared);

            await next.Execute(context).ConfigureAwait(false);
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
namespace RestaurantService.Core.StateMachines.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Extensions;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Services.Core.Events;

    public class PrepareOrderRequestedActivity :
        Activity<OrderState, PrepareOrder>
    {
        readonly ConsumeContext _context;

        public PrepareOrderRequestedActivity(ConsumeContext context)
        {
            _context = context;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, PrepareOrder> context,
            Behavior<OrderState, PrepareOrder> next)
        {
            var items = context.Data.Items.MapExpectedOrderItems().ToList();
            
            context.Instance.CustomerId = context.Data.CustomerId;
            context.Instance.RestaurantId = context.Data.RestaurantId;
            context.Instance.ExpectedItemCount = items.Count;
            context.Instance.ActualItemCount = 0;
            context.Instance.Items = items;
            context.Instance.Timestamp = DateTime.Now;
            
            // fork each item in order to be prepared
            for (int i = 0; i < context.Data.Items.Length; i++)
            {
                await _context.Publish<PrepareOrderItemRequested>(new
                {
                    context.Data.OrderId,
                    context.Data.RestaurantId,
                    MenuItemId = context.Data.Items[i].Id,
                    context.Data.Items[i].SpecialInstructions
                });
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, PrepareOrder, TException> context,
            Behavior<OrderState, PrepareOrder> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
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
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<OrderState, PrepareOrder> context,
            Behavior<OrderState, PrepareOrder> next)
        {
            context.Instance.ExpectedItemCount = context.Data.Items.Length;
            context.Instance.Items = context.Data.Items.GetExpectedOrderItems().ToList();
            
            for (int i = 0; i < context.Data.Items.Length; i++)
            {
                await _context.Publish<PrepareOrderItemRequested>(new
                {
                    context.Data.OrderId,
                    MenuItemId = context.Data.Items[i].Id,
                    context.Data.Items[i].SpecialInstructions
                });
            }
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderState, PrepareOrder, TException> context, Behavior<OrderState, PrepareOrder> next) where TException : Exception => throw new NotImplementedException();
    }
}
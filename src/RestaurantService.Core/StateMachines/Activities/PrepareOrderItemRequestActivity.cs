namespace RestaurantService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Services.Core.Events;

    public class PrepareOrderItemRequestActivity :
        Activity<OrderItemState, PrepareOrderItemRequested>
    {
        readonly ConsumeContext _context;

        public PrepareOrderItemRequestActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<OrderItemState, PrepareOrderItemRequested> context,
            Behavior<OrderItemState, PrepareOrderItemRequested> next)
        {
            await _context.Send<PrepareOrderItem>(new
            {
                context.Data.OrderId,
                context.Data
            });
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderItemState, PrepareOrderItemRequested, TException> context, Behavior<OrderItemState, PrepareOrderItemRequested> next) where TException : Exception => throw new NotImplementedException();
    }
}
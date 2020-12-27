namespace RestaurantService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Services.Core;
    using Services.Core.Events;

    public class OrderReceiptActivity :
        Activity<RestaurantState, OrderReceived>
    {
        readonly ConsumeContext _context;

        public OrderReceiptActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<RestaurantState, OrderReceived> context,
            Behavior<RestaurantState, OrderReceived> next)
        {
            await _context.Publish<OrderReceiptConfirmed>(new
            {
                context.Data.OrderId,
                context.Data.CustomerId,
                context.Data.RestaurantId,
                context.Data.Items
            });
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<RestaurantState, OrderReceived, TException> context,
            Behavior<RestaurantState, OrderReceived> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
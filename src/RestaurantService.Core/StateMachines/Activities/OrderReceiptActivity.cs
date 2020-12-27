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
        readonly IOrderManager _manager;

        public OrderReceiptActivity(ConsumeContext context, IOrderManager manager)
        {
            _context = context;
            _manager = manager;
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
            OperationResult result = await _manager.Receive(context.Data);
            
            if (result.OperationPerformed == OperationType.Receipt)
            {
                await _context.Send<ValidateOrder>(new
                {
                    context.Data.OrderId,
                    context.Data.CustomerId,
                    context.Data.Items,
                    context.Data.RestaurantId,
                    Timestamp = DateTimeOffset.Now
                });
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<RestaurantState, OrderReceived, TException> context,
            Behavior<RestaurantState, OrderReceived> next)
            where TException : Exception => await next.Faulted(context);
    }
}
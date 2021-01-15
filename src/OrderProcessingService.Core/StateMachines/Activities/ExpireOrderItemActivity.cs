namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Services.Core.Events;

    public class ExpireOrderItemActivity :
        Activity<OrderItemState, OrderItemExpired>
    {
        readonly ConsumeContext _context;

        public ExpireOrderItemActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemExpired> context,
            Behavior<OrderItemState, OrderItemExpired> next)
        {
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderItemState, OrderItemExpired, TException> context, Behavior<OrderItemState, OrderItemExpired> next) where TException : Exception => throw new NotImplementedException();
    }
}
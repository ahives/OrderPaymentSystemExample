namespace OrderProcessingService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Sagas;
    using Services.Core.Events;

    public class OrderItemExpiredActivity :
        Activity<OrderItemState, OrderItemExceededPreparationLimit>
    {
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<OrderItemState, OrderItemExceededPreparationLimit> context, Behavior<OrderItemState, OrderItemExceededPreparationLimit> next) => throw new NotImplementedException();

        public async Task Faulted<TException>(BehaviorExceptionContext<OrderItemState, OrderItemExceededPreparationLimit, TException> context, Behavior<OrderItemState, OrderItemExceededPreparationLimit> next) where TException : Exception => throw new NotImplementedException();
    }
}
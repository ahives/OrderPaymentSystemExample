namespace OrderReceiptService.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Restaurant.Core;
    using Sagas;

    public class OrderDiscardedActivity :
        Activity<RestaurantState, OrderDiscarded>
    {
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<RestaurantState, OrderDiscarded> context, Behavior<RestaurantState, OrderDiscarded> next) => throw new NotImplementedException();

        public async Task Faulted<TException>(BehaviorExceptionContext<RestaurantState, OrderDiscarded, TException> context, Behavior<RestaurantState, OrderDiscarded> next) where TException : Exception => throw new NotImplementedException();
    }
}
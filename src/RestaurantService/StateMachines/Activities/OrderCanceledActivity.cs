namespace RestaurantService.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Restaurant.Core;
    using Sagas;

    public class OrderCanceledActivity :
        Activity<RestaurantState, OrderCanceled>
    {
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<RestaurantState, OrderCanceled> context, Behavior<RestaurantState, OrderCanceled> next) => throw new NotImplementedException();

        public async Task Faulted<TException>(BehaviorExceptionContext<RestaurantState, OrderCanceled, TException> context, Behavior<RestaurantState, OrderCanceled> next) where TException : Exception => throw new NotImplementedException();
    }
}
namespace RestaurantService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using Restaurant.Core;
    using Sagas;

    public class OrderPreparedActivity :
        Activity<RestaurantState, OrderPrepared>
    {
        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<RestaurantState, OrderPrepared> context, Behavior<RestaurantState, OrderPrepared> next) => throw new NotImplementedException();

        public async Task Faulted<TException>(BehaviorExceptionContext<RestaurantState, OrderPrepared, TException> context, Behavior<RestaurantState, OrderPrepared> next) where TException : Exception => throw new NotImplementedException();
    }
}
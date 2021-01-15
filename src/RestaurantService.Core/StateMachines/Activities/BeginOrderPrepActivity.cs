namespace RestaurantService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using Data.Core.Model;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class BeginOrderPrepActivity :
        Activity<RestaurantState, OrderValidated>
    {
        readonly ConsumeContext _context;

        public BeginOrderPrepActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<RestaurantState, OrderValidated> context,
            Behavior<RestaurantState, OrderValidated> next)
        {
            Log.Information($"Courier State Machine - {nameof(BeginOrderPrepActivity)}");

            context.Instance.Timestamp = DateTime.Now;

            try
            {
                await _context.Publish<PrepareOrder>(new
                {
                    context.Data.OrderId,
                    context.Data.CustomerId,
                    context.Data.RestaurantId,
                    context.Data.Items
                });

                // await UpdateOrder(context.Data);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<RestaurantState, OrderValidated, TException> context,
            Behavior<RestaurantState, OrderValidated> next) where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class OrderReadyForPickUpActivity :
        Activity<CourierState, OrderReadyForDelivery>
    {
        readonly ConsumeContext _context;

        public OrderReadyForPickUpActivity(ConsumeContext context)
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

        public async Task Execute(BehaviorContext<CourierState, OrderReadyForDelivery> context,
            Behavior<CourierState, OrderReadyForDelivery> next)
        {
            Log.Information($"Courier State Machine - {nameof(OrderExpiredActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;

            if (context.Instance.IsOrderReady)
            {
                // TODO: might want to fault if the courier Id has not been set at this point
                if (!context.Instance.CourierId.HasValue)
                    return;
                
                await _context.Send<PickUpOrder>(new()
                {
                    CourierId = context.Instance.CourierId.Value,
                    RestaurantId = context.Data.RestaurantId,
                    CustomerId = context.Data.CustomerId,
                    OrderId = context.Data.OrderId
                });
            }
            else
            {
                // TODO: schedule a wait
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, OrderReadyForDelivery, TException> context,
            Behavior<CourierState, OrderReadyForDelivery> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
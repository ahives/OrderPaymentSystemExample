namespace OrderReceiptService.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Data.Core;
    using Data.Core.Model;
    using GreenPipes;
    using MassTransit;
    using Restaurant.Core;
    using Sagas;

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
            throw new NotImplementedException();
        }

        public void Accept(StateMachineVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public async Task Execute(BehaviorContext<RestaurantState, OrderValidated> context,
            Behavior<RestaurantState, OrderValidated> next)
        {
            try
            {
                context.Instance.Timestamp = DateTime.Now;

                _context.Publish<PrepareOrder>(new
                {
                    context.Data.OrderId,
                    context.Data.CustomerId,
                    context.Data.RestaurantId,
                    context.Data.Items,
                    Timestamp = DateTime.Now
                });

                await UpdateOrder(context.Data);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<RestaurantState, OrderValidated, TException> context, Behavior<RestaurantState, OrderValidated> next) where TException : Exception => throw new NotImplementedException();

        async Task UpdateOrder(OrderValidated data)
        {
            await using DatabaseContext db = new DatabaseContext();

            Order order = await db.Orders.FindAsync(data.OrderId);

            if (order != null)
            {
                order.Status = OrderStatus.BeingPrepared;
                order.StatusTimestamp = DateTime.Now;

                await db.SaveChangesAsync();
                
                var delay = GetRandomDelay();

                _context.ScheduleSend<DispatchCourier>(delay, new
                {
                    data.OrderId,
                    data.CustomerId,
                    data.Items,
                    data.RestaurantId,
                    Timestamp = DateTime.Now
                });
            }
        }

        TimeSpan GetRandomDelay()
        {
            Random random = new Random();
            int delayInSeconds = random.Next(2, 6);
            TimeSpan delay = new TimeSpan(0, 0, 0, delayInSeconds);
            
            return delay;
        }
    }
}
namespace RestaurantService.Core.StateMachines.Activities
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

    public class OrderReceivedActivity :
        Activity<RestaurantState, OrderReceived>
    {
        readonly ConsumeContext _context;
        readonly OrdersDbContext _db;

        public OrderReceivedActivity(ConsumeContext context, OrdersDbContext db)
        {
            _context = context;
            _db = db;
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
            try
            {
                await _context.Send<ValidateOrder>(new
                {
                    context.Data.OrderId,
                    context.Data.CustomerId,
                    context.Data.Items,
                    context.Data.RestaurantId,
                    Timestamp = DateTimeOffset.Now
                });

                await SaveOrder(context.Data);

                await next.Execute(context).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // fault
                throw;
            }
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<RestaurantState, OrderReceived, TException> context,
            Behavior<RestaurantState, OrderReceived> next)
            where TException : Exception => await next.Faulted(context);

        async Task SaveOrder(OrderReceived data)
        {
            await _db.Orders.AddAsync(new Order
            {
                OrderId = data.OrderId,
                CustomerId = data.CustomerId,
                CourierId = null,
                Status = OrderStatus.New,
                CreationTimestamp = DateTime.Now
            });

            for (int i = 0; i < data.Items.Length; i++)
            {
                await _db.OrderItems.AddAsync(new OrderItem
                {
                    OrderItemId = NewId.NextGuid(),
                    OrderId = data.OrderId,
                    MenuItemId = data.Items[i],
                    CreationTimestamp = DateTime.Now
                });
            }

            await _db.SaveChangesAsync();
        }
    }
}
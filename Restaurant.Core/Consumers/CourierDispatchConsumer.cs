namespace Restaurant.Core
{
    using System;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;

    public class CourierDispatchConsumer :
        IConsumer<DispatchCourier>
    {
        public async Task Consume(ConsumeContext<DispatchCourier> context)
        {
            try
            {
                context.Publish<CourierDispatched>(new
                {
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId,
                    context.Message.Items,
                    Timestamp = DateTime.Now
                });

                await UpdateOrder(context.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        async Task UpdateOrder(DispatchCourier data)
        {
            await using DatabaseContext db = new DatabaseContext();

            Order order = await db.Orders.FindAsync(data.OrderId);

            if (order != null)
            {
                Courier courier = await db.Couriers
                    .FirstOrDefaultAsync(x => x.RegionId == order.RegionId && x.IsAvailable);;

                if (courier == null)
                {
                    return;
                }
                
                order.CourierId = courier.CourierId;

                courier.IsAvailable = false;
                
                await db.SaveChangesAsync();
            }
        }
    }
}
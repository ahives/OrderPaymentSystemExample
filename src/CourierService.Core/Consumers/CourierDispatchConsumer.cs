namespace CourierService.Core.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Services.Core.Events;

    public class CourierDispatchConsumer :
        IConsumer<DispatchCourier>
    {
        public CourierDispatchConsumer()
        {
        }

        public async Task Consume(ConsumeContext<DispatchCourier> context)
        {
            try
            {
                await context.Publish<CourierDispatched>(new
                {
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });

                // await UpdateOrder(context.Message);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
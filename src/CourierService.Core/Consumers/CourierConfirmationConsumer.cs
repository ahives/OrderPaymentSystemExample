namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core;
    using Services.Core.Events;

    public class CourierConfirmationConsumer :
        IConsumer<ConfirmCourier>
    {
        readonly ICourierFinder _finder;

        public CourierConfirmationConsumer(ICourierFinder finder)
        {
            _finder = finder;
        }

        public async Task Consume(ConsumeContext<ConfirmCourier> context)
        {
            var result = await _finder.Find(new CourierFinderRequest
            {
                Street = context.Message.Street,
                City = context.Message.City,
                RegionId = context.Message.RegionId,
                ZipCode = context.Message.ZipCode
            });
            
            if (result.IsSuccessful)
            {
                await context.Publish<CourierConfirmed>(new
                {
                    CourierId = result.Value,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId
                });
            }
        }
    }
}
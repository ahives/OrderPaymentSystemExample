namespace OrderReceiptService.Consumers
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Restaurant.Core;

    public class OrderValidationConsumer :
        IConsumer<ValidateOrder>
    {
        readonly IOrderValidator _validator;

        public OrderValidationConsumer(IOrderValidator validator)
        {
            _validator = validator;
        }

        public async Task Consume(ConsumeContext<ValidateOrder> context)
        {
            bool isValid = _validator.Validate(context.Message);

            if (isValid)
            {
                await context.Publish<OrderValidated>(new
                {
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId,
                    context.Message.Items,
                    Timestamp = DateTime.Now
                });
            }
            else
            {
                await context.Publish<OrderNotValidated>(new
                {
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.RestaurantId,
                    context.Message.Items,
                    Timestamp = DateTime.Now
                });
            }
        }
    }
}
namespace RestaurantService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Services.Core;
    using Services.Core.Events;

    public class ReceiptConfirmationConsumer :
        IConsumer<OrderReceiptConfirmed>
    {
        readonly IOrderManager _manager;

        public ReceiptConfirmationConsumer(IOrderManager manager)
        {
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<OrderReceiptConfirmed> context)
        {
            OperationResult result = await _manager.Receive(context.Message);
            
            if (result.OperationPerformed == OperationType.Receipt)
            {
                await context.Send<ValidateOrder>(new
                {
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    context.Message.Items,
                    context.Message.RestaurantId
                });
            }
        }
    }
}
namespace OrderProcessingService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Services.Core.Events;

    public class OrderNotFoundConsumer :
        IConsumer<OrderNotFound>
    {
        readonly ILogger<OrderNotFoundConsumer> _logger;

        public OrderNotFoundConsumer(ILogger<OrderNotFoundConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderNotFound> context)
        {
            _logger.LogInformation($"Consumer - {nameof(OrderNotFoundConsumer)} => consumed {nameof(OrderNotFound)} event");
        }
    }
}
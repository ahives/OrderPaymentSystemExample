namespace OrderProcessingWebService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Services.Core.Events;

    [ApiController]
    [Route("[controller]")]
    public class OrderProcessingController :
        ControllerBase
    {
        readonly IPublishEndpoint _endpoint;
        readonly ILogger<OrderProcessingController> _logger;

        public OrderProcessingController(IPublishEndpoint endpoint, ILogger<OrderProcessingController> logger)
        {
            _endpoint = endpoint;
            _logger = logger;
        }

        [HttpPost("PrepareOrder")]
        public async Task<IActionResult> PrepareOrder(OrderPreparationContext context)
        {
            Guid orderId = context.OrderId == Guid.Empty ? NewId.NextGuid() : context.OrderId;

            await _endpoint.Publish<RequestOrderPreparation>(
                new()
                {
                    OrderId = orderId,
                    CustomerId = context.CustomerId,
                    RestaurantId = context.RestaurantId,
                    AddressId = context.AddressId,
                    Items = context.Items
                });

            _logger.LogInformation($"Published - {nameof(RequestOrderPreparation)}");

            return Ok(orderId);
        }

        [HttpPost("OrderReadyForDelivery")]
        public async Task<IActionResult> OrderReadyForDelivery(OrderReadyContext context)
        {
            await _endpoint.Publish<OrderReadyForDelivery>(
                new()
                {
                    OrderId = context.OrderId,
                    CustomerId = context.CustomerId,
                    RestaurantId = context.RestaurantId
                });

            _logger.LogInformation($"Published - {nameof(OrderReadyForDelivery)}");

            return Ok();
        }

        [HttpPost("OrderExpired")]
        public async Task<IActionResult> OrderExpired(OrderExpiredContext context)
        {
            await _endpoint.Publish<OrderExpired>(
                new()
                {
                    OrderId = context.OrderId,
                    CustomerId = context.CustomerId,
                    RestaurantId = context.RestaurantId
                });

            _logger.LogInformation($"Published - {nameof(OrderExpired)}");

            return Ok();
        }

        [HttpPost("CancelOrderRequest")]
        public async Task<IActionResult> CancelOrderRequest(CancelOrderContext context)
        {
            await _endpoint.Publish<OrderCancelRequest>(
                new()
                {
                    CourierId = context.CourierId,
                    OrderId = context.OrderId,
                    CustomerId = context.CustomerId,
                    RestaurantId = context.RestaurantId
                });

            _logger.LogInformation($"Published - {nameof(OrderCancelRequest)}");

            return Ok();
        }

        [HttpPost("VoidOrderItemRequest")]
        public async Task<IActionResult> VoidOrderItemRequest(VoidOrderItemRequestContext context)
        {
            await _endpoint.Publish<VoidOrderItemRequest>(
                new()
                {
                    OrderId = context.OrderId,
                    OrderItemId = context.OrderItemId,
                    CustomerId = context.CustomerId,
                    RestaurantId = context.RestaurantId
                });

            _logger.LogInformation($"Published - {nameof(VoidOrderItemRequest)}");

            return Ok();
        }

        [HttpPost("CancelOrderItemRequest")]
        public async Task<IActionResult> CancelOrderItemRequest(CancelOrderItemContext context)
        {
            await _endpoint.Publish<OrderItemCancelRequest>(
                new()
                {
                    OrderId = context.OrderId,
                    OrderItemId = context.OrderItemId
                });

            _logger.LogInformation($"Published - {nameof(OrderItemCancelRequest)}");

            return Ok();
        }

        // [HttpPost("CancelOrder")]
        // public async Task<IActionResult> OrderCanceled(CancelOrderContext context)
        // {
        //     await _endpoint.Publish<OrderCanceled>(new()
        //     {
        //         CourierId = context.CourierId,
        //         OrderId = context.OrderId,
        //         CustomerId = context.CustomerId,
        //         RestaurantId = context.RestaurantId
        //     });
        //
        //     return Ok();
        // }

        [HttpPost("DiscardOrderItem")]
        public async Task<IActionResult> OrderItemDiscarded(OrderDiscardContext context)
        {
            await _endpoint.Publish<OrderItemDiscarded>(
                new()
                {
                    OrderId = context.OrderId,
                    OrderItemId = context.OrderItemId
                });

            _logger.LogInformation($"Published - {nameof(OrderItemDiscarded)}");

            return Ok();
        }
    }
}
namespace OrderProcessingWebService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using OrderProcessingWbService;
    using Services.Core.Events;

    [ApiController]
    [Route("[controller]")]
    public class OrderProcessingController :
        ControllerBase
    {
        readonly IPublishEndpoint _endpoint;

        public OrderProcessingController(IPublishEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        [HttpPost("PrepareOrder")]
        public async Task<IActionResult> PrepareOrder(RequestOrderPreparation request)
        {
            Guid orderId = NewId.NextGuid();
            
            await _endpoint.Publish<RequestOrderPreparation>(new()
            {
                OrderId = orderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId,
                AddressId = request.AddressId,
                Items = request.Items
            });

            return Ok(orderId);
        }

        [HttpPost("OrderReadyForDelivery")]
        public async Task<IActionResult> OrderReadyForDelivery(OrderReadyRequest request)
        {
            await _endpoint.Publish<OrderReadyForDelivery>(new()
            {
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("OrderExpired")]
        public async Task<IActionResult> OrderExpired(OrderExpiredRequest request)
        {
            await _endpoint.Publish<OrderExpired>(new()
            {
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> OrderCanceled(OrderCancelRequest request)
        {
            await _endpoint.Publish<OrderCanceled>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("DiscardOrderItem")]
        public async Task<IActionResult> OrderItemDiscarded(OrderDiscardRequest request)
        {
            await _endpoint.Publish<OrderItemDiscarded>(new()
            {
                OrderId = request.OrderId,
                OrderItemId = request.OrderItemId
            });

            return Ok();
        }
    }
}
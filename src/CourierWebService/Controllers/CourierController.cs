namespace CourierWebService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using Services.Core.Events;

    [ApiController]
    [Route("[controller]")]
    public class CourierController :
        ControllerBase
    {
        readonly IPublishEndpoint _endpoint;
        readonly OrdersDbContext _db;

        public CourierController(IPublishEndpoint endpoint, OrdersDbContext db)
        {
            _endpoint = endpoint;
            _db = db;
        }

        [HttpPost("DispatchCourier")]
        public async Task<IActionResult> DispatchCourier(CourierRequest request)
        {
            await _endpoint.Publish<CourierDispatched>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("DeliveringOrder")]
        public async Task<IActionResult> DeliveringOrder(CourierRequest request)
        {
            await _endpoint.Publish<DeliveringOrder>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("ArrivedAtRestaurant")]
        public async Task<IActionResult> ArrivedAtRestaurant(CourierRequest request)
        {
            await _endpoint.Publish<CourierArrivedAtRestaurant>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("OrderReadyForDelivery")]
        public async Task<IActionResult> OrderReadyForDelivery(CourierRequest request)
        {
            await _endpoint.Publish<OrderReadyForDelivery>(new()
            {
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("DispatchConfirmed")]
        public async Task<IActionResult> DispatchConfirmed(CourierRequest request)
        {
            await _endpoint.Publish<CourierDispatchConfirmed>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("EnRouteToRestaurant")]
        public async Task<IActionResult> EnRouteToRestaurant(CourierRequest request)
        {
            await _endpoint.Publish<CourierEnRouteToRestaurant>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("EnRouteToCustomer")]
        public async Task<IActionResult> EnRouteToCustomer(CourierRequest request)
        {
            await _endpoint.Publish<CourierEnRouteToCustomer>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }
    }

    public record CourierRequest
    {
        public Guid CourierId { get; init; }
        
        public Guid OrderId { get; init; }
        
        public Guid CustomerId { get; init; }
        
        public Guid RestaurantId { get; init; }
    }
}
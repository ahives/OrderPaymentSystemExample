namespace CourierWebService.Controllers
{
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

        [HttpPost("RequestCourierDispatch")]
        public async Task<IActionResult> RequestCourierDispatch(DispatchRequest request)
        {
            await _endpoint.Publish<RequestCourierDispatch>(new()
            {
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("DispatchConfirmed")]
        public async Task<IActionResult> DispatchConfirmed(ChangeStatusRequest request)
        {
            await _endpoint.Publish<ConfirmCourierDispatch>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("EnRouteToRestaurant")]
        public async Task<IActionResult> EnRouteToRestaurant(ChangeStatusRequest request)
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

        [HttpPost("ArrivedAtRestaurant")]
        public async Task<IActionResult> ArrivedAtRestaurant(ChangeStatusRequest request)
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

        [HttpPost("EnRouteToCustomer")]
        public async Task<IActionResult> EnRouteToCustomer(ChangeStatusRequest request)
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

        [HttpPost("ArrivedAtCustomer")]
        public async Task<IActionResult> ArrivedAtCustomer(ChangeStatusRequest request)
        {
            await _endpoint.Publish<CourierArrivedAtCustomer>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }

        [HttpPost("DeliveringOrder")]
        public async Task<IActionResult> DeliveringOrder(ChangeStatusRequest request)
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

        [HttpPost("DeclineDispatchRequest")]
        public async Task<IActionResult> DeclineDispatchRequest(DeclineDispatchRequest request)
        {
            await _endpoint.Publish<CourierDispatchDeclined>(new()
            {
                CourierId = request.CourierId,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId
            });

            return Ok();
        }
    }
}
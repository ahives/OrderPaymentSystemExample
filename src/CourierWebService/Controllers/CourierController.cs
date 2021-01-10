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
        readonly ISendEndpoint _endpoint;
        readonly OrdersDbContext _db;
        Guid _courierId;
        Guid _orderId;
        Guid _customerId;
        Guid _restaurantId;

        public CourierController(ISendEndpoint endpoint, OrdersDbContext db)
        {
            _endpoint = endpoint;
            _db = db;
        }

        [HttpPost("EnRouteToRestaurant")]
        public async Task EnRouteToRestaurant()
        {
            await _endpoint.Send<CourierArrivedAtRestaurant>(new()
            {
                CourierId = _courierId,
                OrderId = _orderId,
                CustomerId = _customerId,
                RestaurantId = _restaurantId
            });
        }

        [HttpPost("EnRouteToCustomer")]
        public async Task EnRouteToCustomer()
        {
            await _endpoint.Send<CourierEnRouteToCustomer>(new()
            {
                CourierId = _courierId,
                OrderId = _orderId,
                CustomerId = _customerId,
                RestaurantId = _restaurantId
            });
        }
        
        // GET
        // public IActionResult Index()
        // {
        //     return View();
        // }
    }
}
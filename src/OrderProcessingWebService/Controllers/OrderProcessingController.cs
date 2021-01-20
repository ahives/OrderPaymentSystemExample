namespace OrderProcessingWebService.Controllers
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
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
    }
}
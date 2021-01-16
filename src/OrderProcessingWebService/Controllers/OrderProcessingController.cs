namespace OrderProcessingWebService.Controllers
{
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
        public async Task<IActionResult> PrepareOrder(PrepareOrderRequest request)
        {
            await _endpoint.Publish<PrepareOrder>(new()
            {
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId,
                Items = request.Items
            });

            return Ok();
        }
    }
}
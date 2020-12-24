namespace CurationService.Consumers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using MassTransit;
    using Restaurant.Core.Events;
    using Serilog;

    public class OrderCurationConsumer :
        IConsumer<CurateOrders>
    {
        readonly OrdersDbContext _db;

        public OrderCurationConsumer(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task Consume(ConsumeContext<CurateOrders> context)
        {
            var orderItems = _db.OrderItems.Where(x => x.IsExpired).ToList();

            if (orderItems.Any())
            {
                for (int i = 0; i < orderItems.Count; i++)
                {
                    Log.Information($"{orderItems[i].OrderId} has expired.");
                }
            }
            
            Log.Information(string.Empty);
        }
    }
}
namespace Service.Grpc.Core
{
    using System;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Model;

    public class OrderProcessor :
        IOrderProcessor
    {
        readonly OrdersDbContext _db;

        public OrderProcessor(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<Result<OrderItem>> PrepareItem(OrderPrepRequest request)
        {
            var target = await _db.OrderItems.FindAsync(request.OrderItemId);

            if (target != null)
                return new Result<OrderItem> {Value = null, ChangeCount = 0, IsSuccessful = false};
            
            var entity = MapRequest(request);
                
            await _db.OrderItems.AddAsync(entity);

            int changes = await _db.SaveChangesAsync();

            if (changes <= 0)
                return new Result<OrderItem> {Value = null, ChangeCount = changes, IsSuccessful = false};

            var mapped = MapEntity(entity);
                
            return new Result<OrderItem> {Value = mapped, ChangeCount = changes, IsSuccessful = true};
        }

        OrderItem MapEntity(OrderItemEntity entity) =>
            new()
            {
                OrderId = entity.OrderId,
                OrderItemId = entity.OrderItemId,
                MenuItemId = entity.MenuItemId,
                ShelfId = entity.ShelfId,
                Status = entity.Status,
                StatusTimestamp = entity.StatusTimestamp,
                SpecialInstructions = entity.SpecialInstructions,
                TimePrepared = entity.TimePrepared,
                ExpiryTimestamp = entity.ExpiryTimestamp,
                ShelfLife = entity.ShelfLife,
                CreationTimestamp = entity.CreationTimestamp
            };

        OrderItemEntity MapRequest(OrderPrepRequest request) =>
            new()
            {
                OrderItemId = request.OrderItemId,
                OrderId = request.OrderId,
                MenuItemId = request.MenuItemId,
                SpecialInstructions = request.SpecialInstructions,
                Status = (int)OrderItemStatus.Prepared,
                StatusTimestamp = DateTime.Now,
                TimePrepared = DateTime.Now,
                ExpiryTimestamp = null,
                CreationTimestamp = DateTime.Now
            };

        // public async IAsyncEnumerable<Result> Expire()
        // {
        //     var orderItems = _db.OrderItems
        //         .Where(x => x.Status == (int)OrderItemStatus.Prepared)
        //         .ToList();
        //     
        //     for (int i = 0; i < orderItems.Count; i++)
        //     {
        //         ShelfEntity shelf = await _db.Shelves.FindAsync(orderItems[i].ShelfId);
        //
        //         if (shelf != null)
        //         {
        //             TimeSpan age = orderItems[i].ExpiryTimestamp != null
        //                 ? (DateTime.Now - orderItems[i].ExpiryTimestamp).Value
        //                 : (DateTime.Now - orderItems[i].TimePrepared).Value;
        //             
        //             orderItems[i].ShelfLife -= shelf.DecayRate * age.Seconds;
        //             orderItems[i].ExpiryTimestamp = DateTime.Now;
        //
        //             _db.Update(orderItems[i]);
        //
        //             if (orderItems[i].ShelfLife <= 0)
        //             {
        //                 yield return new Result
        //                 {
        //                     Reason = ReasonType.ExpiredOrder
        //                 };
        //             }
        //         }
        //     }
        //
        //     await _db.SaveChangesAsync();
        // }
    }
}
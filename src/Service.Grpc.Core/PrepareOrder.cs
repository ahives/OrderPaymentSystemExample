namespace Service.Grpc.Core
{
    using System;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Model;

    public class PrepareOrder :
        IPrepareOrder
    {
        readonly OrdersDbContext _db;

        public PrepareOrder(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<Result<OrderItem>> Prepare(OrderPrepCriteria criteria)
        {
            var target = await _db.OrderItems.FindAsync(criteria.OrderItemId);

            if (target != null)
                return new Result<OrderItem> {Value = null, ChangeCount = 0, IsSuccessful = false};
            
            var entity = MapRequest(criteria);
                
            await _db.OrderItems.AddAsync(entity);

            int changes = await _db.SaveChangesAsync();

            if (changes <= 0)
                return new Result<OrderItem> {Value = null, ChangeCount = changes, IsSuccessful = false};

            var mapped = MapEntity(entity);
                
            return new Result<OrderItem>
            {
                Value = mapped,
                ChangeCount = changes,
                IsSuccessful = true
            };
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

        OrderItemEntity MapRequest(OrderPrepCriteria criteria) =>
            new()
            {
                OrderItemId = criteria.OrderItemId,
                OrderId = criteria.OrderId,
                MenuItemId = criteria.MenuItemId,
                SpecialInstructions = criteria.SpecialInstructions,
                Status = (int)OrderItemStatus.Prepared,
                StatusTimestamp = DateTime.Now,
                TimePrepared = DateTime.Now,
                ExpiryTimestamp = null,
                CreationTimestamp = DateTime.Now
            };
    }
}
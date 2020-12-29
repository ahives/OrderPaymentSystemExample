namespace Services.Core
{
    using System;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using MassTransit;
    using Model;

    public class Cook :
        ICook
    {
        readonly OrdersDbContext _db;

        public Cook(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<Result<OrderItem>> Prepare(OrderPrepRequest request)
        {
            var target = await _db.OrderItems.FindAsync(request.OrderItemId);

            if (target != null)
                return new Result<OrderItem> {Value = null, ChangeCount = 0, IsSuccessful = false};
            
            var entity = MapRequest(request);
                
            await _db.OrderItems.AddAsync(entity);

            int changes = await _db.SaveChangesAsync();
                
            return new Result<OrderItem>
            {
                Value = MapEntity(entity),
                OperationPerformed = OperationType.None,
                ChangeCount = changes,
                IsSuccessful = false
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

        OrderItemEntity MapRequest(OrderPrepRequest request) =>
            new()
            {
                OrderItemId = NewId.NextGuid(),
                OrderId = request.OrderId,
                MenuItemId = request.MenuItemId,
                SpecialInstructions = request.SpecialInstructions,
                Status = OrderItemStatus.Prepared,
                StatusTimestamp = DateTime.Now,
                TimePrepared = DateTime.Now,
                CreationTimestamp = DateTime.Now
            };
    }
}
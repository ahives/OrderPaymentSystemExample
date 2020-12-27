namespace Services.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using MassTransit;

    public class OrderManager :
        IOrderManager
    {
        readonly OrdersDbContext _db;

        public OrderManager(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<OperationResult> Save(OperationContext<OrderPayload> context)
        {
            await _db.Orders.AddAsync(new Order
            {
                OrderId = context.Payload.OrderId,
                CustomerId = context.Payload.CustomerId,
                RestaurantId = context.Payload.RestaurantId,
                CourierId = null,
                Status = OrderStatus.New,
                StatusTimestamp = DateTime.Now,
                Street = context.Payload.Street,
                City = context.Payload.City,
                RegionId = context.Payload.RegionId,
                ZipCode = context.Payload.ZipCode,
                CreationTimestamp = DateTime.Now
            });

            for (int i = 0; i < context.Payload.Items.Length; i++)
            {
                await _db.OrderItems.AddAsync(new OrderItem
                {
                    OrderItemId = NewId.NextGuid(),
                    OrderId = context.Payload.OrderId,
                    MenuItemId = context.Payload.Items[i].Id,
                    SpecialInstructions = context.Payload.Items[i].SpecialInstructions,
                    Status = OrderItemStatus.New,
                    StatusTimestamp = DateTime.Now,
                    CreationTimestamp = DateTime.Now
                });
            }

            int changes = await _db.SaveChangesAsync();
            
            return new OperationResult
            {
                ChangeCount = changes,
                IsSuccessful = changes > 0,
                OperationPerformed = OperationType.Save,
                Timestamp = DateTime.Now
            };
        }

        public async Task<OperationResult> Prepare(OperationContext<OrderItemStatusPayload> context)
        {
            var order = await _db.OrderItems.FindAsync(context.Payload.OrderId);

            if (order != null)
            {
                // determine if shelf is available
                Shelf shelf = await GetShelf(order.MenuItemId);
                bool isReady = IsReady(shelf);
                
                if (isReady)
                {
                    order.ShelfId = shelf.ShelfId;
                    order.Status = context.Payload.Status;
                    order.StatusTimestamp = DateTime.Now;
                }
            }

            int changes = await _db.SaveChangesAsync();

            return new OperationResult();
        }

        bool IsReady(Shelf shelf)
        {
            var orderItems = from orderItem in _db.OrderItems
                where orderItem.Status == OrderItemStatus.Prepared && orderItem.ShelfId == shelf.ShelfId
                select orderItem;

            return orderItems.Count() < shelf.Capacity;
        }

        async Task<Shelf> GetShelf(Guid menuItemId)
        {
            Shelf result = (from menuItem in _db.MenuItems
                    from shelf in _db.Shelves
                    where menuItem.MenuItemId == menuItemId &&
                        menuItem.StorageTemperatureId == shelf.StorageTemperatureId
                    select shelf)
                .FirstOrDefault();

            return result;
        }
    }
}
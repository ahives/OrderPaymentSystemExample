namespace Services.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Events;
    using MassTransit;

    public class OrderManager :
        IOrderManager
    {
        readonly OrdersDbContext _db;

        public OrderManager(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<OperationResult> Receive(OrderReceived data)
        {
            await _db.Orders.AddAsync(new Order
            {
                OrderId = data.OrderId,
                CustomerId = data.CustomerId,
                RestaurantId = data.RestaurantId,
                CourierId = null,
                Status = OrderStatus.Receipt,
                StatusTimestamp = DateTime.Now,
                Street = data.Street,
                City = data.City,
                RegionId = data.RegionId,
                ZipCode = data.ZipCode,
                CreationTimestamp = DateTime.Now
            });

            for (int i = 0; i < data.Items.Length; i++)
            {
                var entityEntry = await _db.OrderItems.AddAsync(new OrderItem
                {
                    OrderItemId = NewId.NextGuid(),
                    OrderId = data.OrderId,
                    MenuItemId = data.Items[i].Id,
                    SpecialInstructions = data.Items[i].SpecialInstructions,
                    Status = OrderItemStatus.Receipt,
                    StatusTimestamp = DateTime.Now,
                    CreationTimestamp = DateTime.Now
                });
            }

            int changes = await _db.SaveChangesAsync();
            
            return new OperationResult
            {
                ChangeCount = changes,
                IsSuccessful = changes > 0,
                OperationPerformed = OperationType.Receipt
            };
        }

        public async Task<OperationResult> Prepare(PrepareOrderItem data)
        {
            var order = await _db.OrderItems.FindAsync(data.OrderId);

            if (order == null)
            {
                var result = await _db.OrderItems.AddAsync(new OrderItem
                {
                    OrderItemId = NewId.NextGuid(),
                    OrderId = data.OrderId,
                    MenuItemId = data.MenuItemId,
                    SpecialInstructions = data.SpecialInstructions,
                    Status = OrderItemStatus.Receipt,
                    StatusTimestamp = DateTime.Now,
                    CreationTimestamp = DateTime.Now
                });

                order = result.Entity;

                if (order == null)
                {
                    return new OperationResult
                    {
                        OperationPerformed = OperationType.None,
                        ChangeCount = 0,
                        IsSuccessful = false
                    };
                }
            }

            Shelf shelf = GetShelf(order.MenuItemId);

            if (!IsShelfAvailable(shelf))
            {
                return new OperationResult
                {
                    OperationPerformed = OperationType.None,
                    ChangeCount = 0,
                    IsSuccessful = false
                };
            }

            order.ShelfId = shelf.ShelfId;
            order.Status = OrderItemStatus.Prepared;
            order.StatusTimestamp = DateTime.Now;

            _db.Update(order);

            int changes = await _db.SaveChangesAsync();

            return new OperationResult
            {
                OperationPerformed = OperationType.MovedToShelf,
                ChangeCount = changes,
                IsSuccessful = true
            };
        }

        bool IsShelfAvailable(Shelf shelf)
        {
            var orderItems = from orderItem in _db.OrderItems
                where orderItem.Status == OrderItemStatus.Prepared && orderItem.ShelfId == shelf.ShelfId
                select orderItem;

            return orderItems.Count() < shelf.Capacity;
        }

        Shelf GetShelf(Guid menuItemId)
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
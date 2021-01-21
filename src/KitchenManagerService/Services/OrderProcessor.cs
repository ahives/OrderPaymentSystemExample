namespace KitchenManagerService.Services
{
    using System;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Serilog;
    using Service.Grpc.Core;
    using Service.Grpc.Core.Model;

    public class OrderProcessor :
        IOrderProcessor
    {
        readonly OrdersDbContext _db;

        public OrderProcessor(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<Result<Order>> AddNewOrder(OrderProcessRequest request)
        {
            var entity = CreateOrderEntity(request);
                
            await _db.Orders.AddAsync(entity);

            int changes = await _db.SaveChangesAsync();

            if (changes <= 0)
            {
                Log.Information($"Order {request.OrderId} could not be saved.");
                
                return new Result<Order> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
                
            Log.Information($"Order {request.OrderId} was saved.");
                
            return new Result<Order> {Value = MapOrderEntity(entity), ChangeCount = changes, IsSuccessful = true};
        }

        public async Task<Result<OrderItem>> AddNewOrderItem(OrderPrepRequest request)
        {
            var target = await _db.OrderItems.FindAsync(request.OrderItemId);

            if (target != null)
            {
                Log.Information($"Order item {request.OrderId} already exists.");
                
                return new Result<OrderItem> {Value = null, IsSuccessful = false};
            }
            
            var entity = MapRequest(request);
                
            await _db.OrderItems.AddAsync(entity);

            int changes = await _db.SaveChangesAsync();

            if (changes <= 0)
            {
                Log.Information($"Order item {request.OrderItemId} of Order {request.OrderId} could not be saved.");
                
                return new Result<OrderItem> {Value = null, ChangeCount = changes, IsSuccessful = false};
            }
                
            Log.Information($"Order {request.OrderItemId} was saved.");
                
            return new Result<OrderItem> {Value = MapEntity(entity), ChangeCount = changes, IsSuccessful = true};
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

        Order MapOrderEntity(OrderEntity entity) =>
            new()
            {
                OrderId = entity.OrderId,
                CourierId = entity.CourierId,
                CustomerId = entity.CustomerId,
                RestaurantId = entity.RestaurantId,
                // CustomerPickup = false,
                Status = entity.Status,
                StatusTimestamp = entity.StatusTimestamp
            };

        OrderEntity CreateOrderEntity(OrderProcessRequest request) =>
            new()
            {
                OrderId = request.OrderId,
                CourierId = null,
                CustomerId = request.CustomerId,
                RestaurantId = request.RestaurantId,
                AddressId = request.AddressId,
                CustomerPickup = false,
                Status = (int) OrderStatus.Receipt,
                StatusTimestamp = DateTime.Now,
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
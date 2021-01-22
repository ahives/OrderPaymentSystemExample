namespace KitchenManagerService.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Serilog;
    using Service.Grpc.Core;
    using Service.Grpc.Core.Model;

    public class OrderProcessor :
        IOrderProcessor
    {
        readonly OrdersDbContext _dbContext;

        public OrderProcessor(OrdersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Order>> AddNewOrder(OrderProcessContext context)
        {
            var entity = CreateOrderEntity(context);
                
            await _dbContext.Orders.AddAsync(entity);

            int changes = await _dbContext.SaveChangesAsync();

            if (changes <= 0)
            {
                Log.Information($"Order {context.OrderId} could not be saved.");
                
                return new Result<Order> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
                
            Log.Information($"Order {context.OrderId} was saved.");
                
            return new Result<Order> {Value = MapOrderEntity(entity), ChangeCount = changes, IsSuccessful = true};
        }

        public async Task<Result<OrderItem>> AddNewOrderItem(OrderItemPreparationContext context)
        {
            var target = await _dbContext.OrderItems.FindAsync(context.OrderItemId);

            if (target != null)
            {
                Log.Information($"Order item {context.OrderId} already exists.");
                
                return new Result<OrderItem> {Value = null, IsSuccessful = false};
            }
            
            var entity = MapRequest(context);
                
            await _dbContext.OrderItems.AddAsync(entity);

            int changes = await _dbContext.SaveChangesAsync();

            if (changes <= 0)
            {
                Log.Information($"Order item {context.OrderItemId} of Order {context.OrderId} could not be saved.");
                
                return new Result<OrderItem> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
                
            Log.Information($"Order {context.OrderItemId} was saved.");
                
            return new Result<OrderItem> {Value = MapToOrderItem(entity), ChangeCount = changes, IsSuccessful = true};
        }

        public async Task<Result<ExpectedOrderItem>> AddExpectedOrderItem(AddExpectedOrderItemContext context)
        {
            var item = await _dbContext.ExpectedOrderItems.FindAsync(context.OrderItemId);

            if (item != null)
            {
                Log.Information($"Order item {context.OrderItemId} already exists.");

                return new Result<ExpectedOrderItem> {Value = null, IsSuccessful = false};
            }
            
            var entity = MapAddExpectedOrderItemContext(context);
                
            await _dbContext.ExpectedOrderItems.AddAsync(entity);

            int changes = await _dbContext.SaveChangesAsync();

            if (changes <= 0)
            {
                Log.Information($"Order item {context.OrderItemId} could not be saved.");

                return new Result<ExpectedOrderItem> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }

            Log.Information($"Order item {context.OrderItemId} was saved.");

            return new Result<ExpectedOrderItem> {Value = MapToExpectedOrderItem(entity), ChangeCount = changes, IsSuccessful = true};
        }

        public async Task<Result<ExpectedOrderItem>> UpdateExpectedOrderItem(ExpectedOrderItemContext context)
        {
            var item = await _dbContext.ExpectedOrderItems.FindAsync(context.OrderItemId);

            if (item == null)
            {
                Log.Information($"Could not find order item {context.OrderItemId}.");

                return new Result<ExpectedOrderItem> {Value = null, IsSuccessful = false};
            }

            item.Status = context.Status;

            _dbContext.ExpectedOrderItems.Update(item);

            int changes = await _dbContext.SaveChangesAsync();

            if (changes <= 0)
            {
                Log.Information($"Order item {context.OrderItemId} could not be saved.");

                return new Result<ExpectedOrderItem> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }

            Log.Information($"Order item {context.OrderItemId} was saved.");

            return new Result<ExpectedOrderItem> {Value = MapToExpectedOrderItem(item), ChangeCount = changes, IsSuccessful = true};
        }

        public async Task<Result<int>> GetExpectedOrderItemCount(ExpectedOrderItemCountContext context)
        {
            var items = _dbContext.ExpectedOrderItems
                .Where(x => x.OrderId == context.OrderId);

            if (!items.Any())
            {
                Log.Information($"Could not find any order items for order {context.OrderId}.");

                return new Result<int> {Reason = ReasonType.None, IsSuccessful = false};
            }
            
            int count = items.Count(x => x.Status == (int) OrderItemStatus.Prepared);

            Log.Information($"Found order items for order {context.OrderId}.");

            return new Result<int> {Value = count, IsSuccessful = true};
        }

        public async Task<Result<OrderItem>> CancelOrder(CancelOrderContext context) => throw new NotImplementedException();

        ExpectedOrderItem MapToExpectedOrderItem(ExpectedOrderItemEntity entity) =>
            new()
            {
                OrderId = entity.OrderId,
                OrderItemId = entity.OrderItemId,
                Status = entity.Status
            };

        OrderItem MapToOrderItem(OrderItemEntity entity) =>
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

        OrderItemEntity MapRequest(OrderItemPreparationContext context) =>
            new()
            {
                OrderItemId = context.OrderItemId,
                OrderId = context.OrderId,
                MenuItemId = context.MenuItemId,
                SpecialInstructions = context.SpecialInstructions,
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

        ExpectedOrderItemEntity MapAddExpectedOrderItemContext(AddExpectedOrderItemContext context) =>
            new()
            {
                OrderId = context.OrderId,
                OrderItemId = context.OrderItemId,
                Status = context.Status
            };

        OrderEntity CreateOrderEntity(OrderProcessContext context) =>
            new()
            {
                OrderId = context.OrderId,
                CourierId = null,
                CustomerId = context.CustomerId,
                RestaurantId = context.RestaurantId,
                AddressId = context.AddressId,
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
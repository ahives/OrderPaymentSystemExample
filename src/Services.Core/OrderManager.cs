namespace Services.Core
{
    using System;
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

        public async Task<OperationResult> Save(SaveContext<OrderPayload> context)
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
                    IsExpired = false,
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
    }
}
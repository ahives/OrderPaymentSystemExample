namespace Services.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.Core;

    public class ExpireOrders :
        IExpireOrders
    {
        readonly OrdersDbContext _db;
        readonly IOrderExpiryCalculator _orderExpiryCalculator;

        public ExpireOrders(OrdersDbContext db, IOrderExpiryCalculator orderExpiryCalculator)
        {
            _db = db;
            _orderExpiryCalculator = orderExpiryCalculator;
        }

        // public async IAsyncEnumerable<Result> Expire()
        // {
        //     var orders = _db.Orders
        //         .Where(x => x.Status == OrderStatus.BeingPrepared);
        //
        //     foreach (var order in orders)
        //     {
        //         var orderItems = _db.OrderItems
        //             .Where(x => x.OrderId == order.OrderId && !x.IsExpired);
        //
        //         foreach (var orderItem in orderItems)
        //         {
        //             orderItem.IsExpired = _orderExpiryCalculator.CalcExpiry(new ExpiryCriteria());
        //             
        //             yield return new Result
        //             {
        //                 Id = orderItem.OrderItemId,
        //                 Timestamp = DateTime.Now
        //             };
        //         }
        //     }
        //
        //     await _db.SaveChangesAsync();
        // }
        public IAsyncEnumerable<Result> Expire() => throw new NotImplementedException();
    }
}
namespace OrderProcessingService.Core.StateMachines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sagas;
    using Services.Core.Model;

    public static class StateMachineExtensions
    {
        public static IEnumerable<ExpectedOrderItem> MapExpectedOrderItems(this Item[] items, Guid orderId) =>
            items.Select(t => new ExpectedOrderItem
            {
                CorrelationId = t.MenuItemId,
                OrderId = orderId,
                Status = t.Status,
                Timestamp = DateTime.Now
            });
    }
}
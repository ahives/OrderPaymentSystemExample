namespace RestaurantService.Core.StateMachines.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sagas;
    using Services.Core.Model;

    public static class StateMachineExtensions
    {
        public static IEnumerable<ExpectedOrderItem> MapExpectedOrderItems(this Item[] items) =>
            items.Select(t => new ExpectedOrderItem
            {
                CorrelationId = t.Id,
                Status = t.Status,
                Timestamp = DateTime.Now
            });
    }
}
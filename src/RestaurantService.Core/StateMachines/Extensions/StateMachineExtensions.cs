namespace RestaurantService.Core.StateMachines.Extensions
{
    using System;
    using System.Collections.Generic;
    using Sagas;
    using Services.Core.Model;

    public static class StateMachineExtensions
    {
        public static IEnumerable<ExpectedOrderItem> GetExpectedOrderItems(this Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                yield return new ExpectedOrderItem
                {
                    CorrelationId = items[i].Id,
                    Status = items[i].Status,
                    Timestamp = DateTime.Now
                };
            }
        }
    }
}
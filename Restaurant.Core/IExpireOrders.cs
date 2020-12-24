namespace Restaurant.Core
{
    using System.Collections.Generic;

    public interface IExpireOrders
    {
        IAsyncEnumerable<Result> Expire();
    }
}
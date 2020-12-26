namespace Services.Core
{
    using System.Collections.Generic;

    public interface IExpireOrders
    {
        IAsyncEnumerable<Result> Expire();
    }
}
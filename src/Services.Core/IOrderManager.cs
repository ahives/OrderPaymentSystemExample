namespace Services.Core
{
    using System.Collections.Generic;

    public interface IOrderManager
    {
        IAsyncEnumerable<Result> Expire();
        
        
    }
}
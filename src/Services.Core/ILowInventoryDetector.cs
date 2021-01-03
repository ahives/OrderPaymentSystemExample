namespace Services.Core
{
    using System.Collections.Generic;

    public interface ILowInventoryDetector
    {
        IAsyncEnumerable<Result<Inventory>> FindAll();
    }
}
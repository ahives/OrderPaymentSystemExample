namespace Services.Core
{
    using System.Collections.Generic;
    using Model;

    public interface ILowInventoryDetector
    {
        IAsyncEnumerable<Result<Inventory>> FindAll();
    }
}
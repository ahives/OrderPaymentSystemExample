namespace Service.Grpc.Core
{
    using System.Collections.Generic;
    using Model;

    public interface ILowInventoryDetector
    {
        IAsyncEnumerable<Result<Inventory>> FindAll();
    }
}
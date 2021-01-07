namespace Services.Core
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Model;

    [ServiceContract]
    public interface IShelfManager
    {
        [OperationContract]
        Task<Result<Shelf>> MoveToShelf(ShelfManagerRequest request);

        [OperationContract]
        Task<Result<Shelf>> MoveToOverflow(ShelfManagerRequest request);
    }
}
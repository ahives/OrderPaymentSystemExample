namespace Services.Core
{
    using System.Threading.Tasks;
    using Model;

    public interface IKitchenManager
    {
        Task<Result<Shelf>> MoveToShelf(ShelfMoveCriteria criteria);

        Task<Result<Shelf>> MoveToOverflow(ShelfMoveCriteria criteria);
    }
}
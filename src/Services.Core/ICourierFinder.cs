namespace Services.Core
{
    using System.Threading.Tasks;
    using Model;

    public interface ICourierFinder
    {
        Task<Result<Courier>> Find(Address address);
    }
}
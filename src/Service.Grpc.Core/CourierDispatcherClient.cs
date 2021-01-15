namespace Service.Grpc.Core
{
    using Microsoft.Extensions.Configuration;

    public class CourierDispatcherClient :
        BaseGrpcClient<ICourierDispatcher>
    {
        public CourierDispatcherClient(IConfiguration configuration)
            : base(configuration)
        {
        }
    }
}
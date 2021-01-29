namespace Service.Grpc.Core
{
    using Configuration;
    using Microsoft.Extensions.Options;

    public class CourierDispatcherClient :
        BaseGrpcClient<ICourierDispatcher>
    {
        public CourierDispatcherClient(IOptions<GrpcClientSettings> options)
            : base(options)
        {
        }
    }
}
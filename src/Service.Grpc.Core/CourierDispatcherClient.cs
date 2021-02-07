namespace Service.Grpc.Core
{
    using Configuration;
    using Microsoft.Extensions.Options;

    public class CourierDispatcherClient :
        BaseGrpcClient<ICourierDispatcher>
    {
        public CourierDispatcherClient(GrpcClientSettings settings)
            : base(settings)
        {
        }
    }
}
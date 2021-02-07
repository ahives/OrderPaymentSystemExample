namespace Service.Grpc.Core
{
    using Configuration;
    using Microsoft.Extensions.Options;

    public class OrderProcessorClient :
        BaseGrpcClient<IOrderProcessor>
    {
        public OrderProcessorClient(GrpcClientSettings settings)
            : base(settings)
        {
        }
    }
}
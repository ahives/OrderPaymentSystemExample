namespace Service.Grpc.Core
{
    using Microsoft.Extensions.Configuration;

    public class OrderProcessorClient :
        BaseGrpcClient<IOrderProcessor>
    {
        public OrderProcessorClient(IConfiguration configuration)
            : base(configuration)
        {
        }
    }
}
namespace Service.Grpc.Core
{
    using global::Grpc.Net.Client;

    public interface IGrpcClient<out TClient>
    {
        TClient Client { get; }
        
        GrpcChannel Channel { get; }
    }
}
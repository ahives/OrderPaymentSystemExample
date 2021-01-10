namespace Service.Grpc.Core
{
    using global::Grpc.Net.Client;

    public interface ICourierDispatcherClient
    {
        ICourierDispatcher Client { get; }
        
        GrpcChannel Channel { get; }
    }
}
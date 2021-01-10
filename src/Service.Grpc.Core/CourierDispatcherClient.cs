namespace Service.Grpc.Core
{
    using global::Grpc.Net.Client;
    using Microsoft.Extensions.Configuration;
    using ProtoBuf.Grpc.Client;

    public class CourierDispatcherClient :
        ICourierDispatcherClient
    {
        readonly string _uri;
        GrpcChannel _channel;

        public CourierDispatcherClient(IConfiguration configuration)
        {
            _uri = configuration?.GetSection("Application")["GrpcClientUri"] ?? "http://localhost:5000";
            _channel = GrpcChannel.ForAddress(_uri);
        }

        public ICourierDispatcher Client
        {
            get
            {
                _channel ??= GrpcChannel.ForAddress(_uri);

                return _channel.CreateGrpcService<ICourierDispatcher>();
            }
        }

        public GrpcChannel Channel => _channel;
    }
}
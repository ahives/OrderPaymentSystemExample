namespace Service.Grpc.Core
{
    using global::Grpc.Net.Client;
    using Microsoft.Extensions.Configuration;
    using ProtoBuf.Grpc.Client;

    public class BaseGrpcClient<TClient> :
        IGrpcClient<TClient>
        where TClient : class
    {
        protected readonly string _uri;
        protected GrpcChannel _channel;

        public BaseGrpcClient(IConfiguration configuration)
        {
            _uri = configuration?.GetSection("Application")
                .GetValue<string>("GrpcClientUri") ?? "http://localhost:5000";
            _channel = GrpcChannel.ForAddress(_uri);
        }

        public virtual TClient Client
        {
            get
            {
                _channel ??= GrpcChannel.ForAddress(_uri);

                return _channel.CreateGrpcService<TClient>();
            }
        }

        public virtual GrpcChannel Channel => _channel;
    }
}
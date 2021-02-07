namespace Service.Grpc.Core
{
    using Configuration;
    using global::Grpc.Net.Client;
    using Microsoft.Extensions.Options;
    using ProtoBuf.Grpc.Client;

    public class BaseGrpcClient<TClient> :
        IGrpcClient<TClient>
        where TClient : class
    {
        protected readonly GrpcClientSettings _settings;
        protected GrpcChannel _channel;

        public BaseGrpcClient(GrpcClientSettings settings)
        {
            _settings = settings;
            _channel = GrpcChannel.ForAddress(_settings.ClientUrl);
        }

        public virtual TClient Client
        {
            get
            {
                _channel ??= GrpcChannel.ForAddress(_settings.ClientUrl);

                return _channel.CreateGrpcService<TClient>();
            }
        }

        public virtual GrpcChannel Channel => _channel;
    }
}
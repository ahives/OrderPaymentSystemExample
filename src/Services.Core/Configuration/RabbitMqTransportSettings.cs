namespace Services.Core.Configuration
{
    public class RabbitMqTransportSettings
    {
        public string Host { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string VirtualHost { get; set; }
        
        public int MessageRetryImmediatePolicy { get; set; }
        
        public int MessageRedeliveryImmediatePolicy { get; set; }
    }
}
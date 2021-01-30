namespace CourierService.Core.Configuration
{
    public class CourierServiceSettings
    {
        public int CourierWaitUponArrivalTimeInSeconds { get; set; }
        
        public int MaxDispatchAttempts { get; set; }
    }
}
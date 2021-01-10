namespace CourierService.Core.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using Serilog;
    using Services.Core.Events;

    public class UpdateCourierStatusConsumer :
        IConsumer<UpdateCourierStatus>
    {
        public async Task Consume(ConsumeContext<UpdateCourierStatus> context)
        {
            Log.Information($"Consumer - {nameof(UpdateCourierStatusConsumer)}");
        }
    }
}
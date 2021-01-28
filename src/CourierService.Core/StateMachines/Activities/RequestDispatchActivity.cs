namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Sagas;
    using Services.Core.Events;

    public class RequestDispatchActivity :
        Activity<CourierState, RequestCourierDispatch>
    {
        readonly ConsumeContext _context;
        readonly IConfiguration _configuration;
        readonly ILogger<RequestDispatchActivity> _logger;

        public RequestDispatchActivity(ConsumeContext context, IConfiguration configuration, ILogger<RequestDispatchActivity> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<CourierState, RequestCourierDispatch> context,
            Behavior<CourierState, RequestCourierDispatch> next)
        {
            _logger.LogInformation($"Courier State Machine - {nameof(RequestDispatchActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.RestaurantId = context.Data.RestaurantId;
            context.Instance.CustomerId = context.Data.CustomerId;
            context.Instance.OrderId = context.Data.OrderId;
            context.Instance.CourierId = null;
            context.Instance.IsOrderReady = false;
            context.Instance.HasCourierArrived = false;
            context.Instance.DispatchAttempts = 1;

            int maxDispatchAttempts = _configuration
                ?.GetSection("Application")
                ?.GetValue<int>("MaxDispatchAttempts") ?? 3;
            
            context.Instance.MaxDispatchAttempts = maxDispatchAttempts;

            await _context.Publish<IdentifyCourierForDispatch>(
                new
                {
                    context.Data.OrderId,
                    context.Data.CustomerId,
                    context.Data.RestaurantId
                });

            _logger.LogInformation($"Published - {nameof(IdentifyCourierForDispatch)}");

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(
            BehaviorExceptionContext<CourierState, RequestCourierDispatch, TException> context,
            Behavior<CourierState, RequestCourierDispatch> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }
    }
}
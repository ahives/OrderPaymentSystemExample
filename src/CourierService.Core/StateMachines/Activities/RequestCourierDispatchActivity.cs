namespace CourierService.Core.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit;
    using Sagas;
    using Serilog;
    using Services.Core.Events;

    public class RequestCourierDispatchActivity :
        Activity<CourierState, RequestCourierDispatch>
    {
        readonly ConsumeContext _context;

        public RequestCourierDispatchActivity(ConsumeContext context)
        {
            _context = context;
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
            Log.Information($"Courier State Machine - {nameof(RequestCourierDispatchActivity)}");
            
            context.Instance.Timestamp = DateTime.Now;
            context.Instance.RestaurantId = context.Data.RestaurantId;
            context.Instance.CustomerId = context.Data.CustomerId;
            context.Instance.OrderId = context.Data.OrderId;
            context.Instance.CourierId = null;
            context.Instance.IsOrderReady = false;
            context.Instance.HasCourierArrived = false;
            
            await _context.Publish<IdentifyCourierForDispatch>(new
            {
                context.Data.OrderId,
                context.Data.CustomerId,
                context.Data.RestaurantId
            });

            Log.Information($"Sent {nameof(IdentifyCourierForDispatch)}");

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
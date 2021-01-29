namespace CourierDispatcherService.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Service.Grpc.Core;
    using Service.Grpc.Core.Model;

    public class CourierDispatcher :
        ICourierDispatcher
    {
        readonly OrdersDbContext _db;
        readonly ILogger<CourierDispatcher> _logger;

        public CourierDispatcher(OrdersDbContext db, ILogger<CourierDispatcher> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Result<Courier>> Identify(CourierIdentificationContext context)
        {
            var customer = await _db.Customers.FindAsync(context.CustomerId);
            
            if (customer == null)
            {
                _logger.LogInformation("Customer not found.");

                return new Result<Courier> {Reason = ReasonType.CustomerNotFound, IsSuccessful = false};
            }
            
            var customerAddress = await _db.Addresses.FindAsync(customer.AddressId);
            
            foreach (var courier in (from courier in _db.Couriers select courier).ToList())
            {
                var address = await _db.Addresses.FindAsync(courier.AddressId);

                if (address == null || address.RegionId != customerAddress.RegionId || address.City != customerAddress.City)
                    continue;

                if (!courier.IsActive || courier.Status != (int)CourierStatus.Idle)
                {
                    Log.Information($"Courier {courier.CourierId} could not be chosen because he/she status is not available.");
                    continue;
                }
            
                _logger.LogInformation($"Courier {courier.CourierId} was identified for dispatch.");
                
                return new Result<Courier> {Value = MapEntity(courier, address), IsSuccessful = true};
            }
            
            _logger.LogInformation("No couriers currently available in the area.");
            
            return new Result<Courier> {Reason = ReasonType.CourierNotAvailable, IsSuccessful = false};
        }

        public async Task<Result<Courier>> Decline(CourierDispatchContext context)
        {
            var courier = await _db.Couriers.FindAsync(context.CourierId);
            
            if (courier == null)
            {
                _logger.LogInformation($"Courier {context.CourierId} could not be found.");
                
                return new Result<Courier> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};
            }
            
            courier.Status = (int)CourierStatus.DispatchDeclined;
            courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(courier);
            
            var order = await _db.Orders.FindAsync(context.OrderId);
            
            if (order == null)
            {
                _logger.LogInformation($"Order {context.OrderId} could not be found.");
                
                return new Result<Courier> {Reason = ReasonType.OrderNotFound, IsSuccessful = false};
            }

            order.CourierId = null;
            order.StatusTimestamp = DateTime.Now;

            _db.Update(order);
            
            var changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
            {
                _logger.LogInformation($"Courier {context.CourierId} was not updated.");
                
                return new Result<Courier> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
            
            var address = await _db.Addresses.FindAsync(courier.AddressId);

            _logger.LogInformation($"Order {context.OrderId} and courier {context.CourierId} information was updated.");
                
            return new Result<Courier> {ChangeCount = changes, Value = MapEntity(courier, address), IsSuccessful = true};
        }

        public async Task<Result<Order>> PickUpOrder(CourierDispatchContext context)
        {
            var restaurant = await _db.Restaurants.FindAsync(context.RestaurantId);
            
            if (restaurant == null || !restaurant.IsActive)
            {
                _logger.LogInformation($"Restaurant {context.RestaurantId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.RestaurantNotFound, IsSuccessful = false};
            }
            
            if (!restaurant.IsOpen)
            {
                _logger.LogInformation($"Restaurant {context.RestaurantId} is not open.");
                
                return new Result<Order> {Reason = ReasonType.RestaurantNotOpen, IsSuccessful = false};
            }

            var courier = await _db.Couriers.FindAsync(context.CourierId);
            
            if (courier == null)
            {
                _logger.LogInformation($"Courier {context.CourierId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};
            }
            
            courier.Status = (int)CourierStatus.PickedUpOrder;
            courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(courier);
            
            var order = await _db.Orders.FindAsync(context.OrderId);
            
            if (order == null)
            {
                _logger.LogInformation($"Order {context.OrderId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.OrderNotFound, IsSuccessful = false};
            }

            order.CourierId = courier.CourierId;
            order.Status = (int) OrderStatus.Delivering;
            order.StatusTimestamp = DateTime.Now;

            _db.Update(order);
            
            int changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
            {
                _logger.LogInformation($"Order {context.OrderId} was not updated.");
                
                return new Result<Order> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
            
            _logger.LogInformation($"Order {context.OrderId} and courier {context.CourierId} information was updated.");
                
            return new Result<Order> {ChangeCount = changes, Value = MapEntity(courier, order), IsSuccessful = true};
        }

        public async Task<Result<Order>> Deliver(CourierDispatchContext context)
        {
            var courier = await _db.Couriers.FindAsync(context.CourierId);
            
            if (courier == null)
            {
                _logger.LogInformation($"Courier {context.CourierId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};
            }
            
            courier.Status = (int)CourierStatus.DeliveredOrder;
            courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(courier);
            
            var order = await _db.Orders.FindAsync(context.OrderId);
            
            if (order == null)
            {
                _logger.LogInformation($"Order {context.OrderId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.OrderNotFound, IsSuccessful = false};
            }

            order.CourierId ??= courier.CourierId;
            order.Status = (int) OrderStatus.Delivered;
            order.StatusTimestamp = DateTime.Now;

            _db.Update(order);
            
            int changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
            {
                _logger.LogInformation($"Order {context.OrderId} was not updated.");
                
                return new Result<Order> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
            
            _logger.LogInformation($"Order {context.OrderId} and courier {context.CourierId} information was updated.");
                
            return new Result<Order> {ChangeCount = changes, Value = MapEntity(courier, order), IsSuccessful = true};
        }

        public async Task<Result<Courier>> ChangeStatus(CourierStatusChangeContext context)
        {
            var courier = await _db.Couriers.FindAsync(context.CourierId);
            
            if (courier == null)
            {
                _logger.LogInformation($"Courier {context.CourierId} could not be found.");
                
                return new Result<Courier> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};
            }
            
            courier.Status = (int)context.Status;
            courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(courier);
            
            int changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
            {
                _logger.LogInformation($"Courier {context.CourierId} status was not updated.");
                
                return new Result<Courier> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
            
            var address = await _db.Addresses.FindAsync(courier.AddressId);

            _logger.LogInformation($"Courier {context.CourierId} status was updated.");
                
            return new Result<Courier> {ChangeCount = changes, Value = MapEntity(courier, address), IsSuccessful = true};
        }

        Order MapEntity(CourierEntity courier, OrderEntity order) =>
            new()
            {
                OrderId = order.OrderId,
                Status = order.Status,
                StatusTimestamp = order.StatusTimestamp,
                RestaurantId = order.RestaurantId,
                CustomerId = order.CustomerId,
                CourierId = courier.CourierId,
                AddressId = order.AddressId
            };

        Courier MapEntity(CourierEntity courier, AddressEntity address) =>
            new()
            {
                CourierId = courier.CourierId,
                Status = courier.Status,
                FirstName = courier.FirstName,
                LastName = courier.LastName,
                Address = new Address
                {
                    Street = address.Street,
                    City = address.City,
                    RegionId = address.RegionId,
                    ZipCode = address.ZipCode
                }
            };
    }
}
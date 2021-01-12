namespace Service.Grpc.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Core;
    using Data.Core.Model;
    using Model;
    using Serilog;

    public class CourierDispatcher :
        ICourierDispatcher
    {
        readonly OrdersDbContext _db;

        public CourierDispatcher(OrdersDbContext db)
        {
            _db = db;
        }

        public async Task<Result<Courier>> Identify(CourierIdentificationRequest request)
        {
            var customer = await _db.Customers.FindAsync(request.CustomerId);
            
            if (customer == null)
            {
                Log.Information("Customer not found.");

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
                    Log.Information($"Courier {courier.CourierId} could be chosen because he/she status is not available.");
                    continue;
                }
            
                Log.Information($"Courier {courier.CourierId} was identified for dispatch.");
                
                return new Result<Courier> {Value = MapEntity(courier, address), IsSuccessful = true};
            }
            
            Log.Information("No couriers currently available in the area.");
            
            return new Result<Courier> {Reason = ReasonType.CourierNotAvailable, IsSuccessful = false};
        }

        public async Task<Result<Courier>> Decline(CourierDispatchRequest request)
        {
            var courier = await _db.Couriers.FindAsync(request.CourierId);
            
            if (courier == null)
            {
                Log.Information($"Courier {request.CourierId} could not be found.");
                
                return new Result<Courier> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};
            }
            
            courier.Status = (int)CourierStatus.Declined;
            courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(courier);
            
            var order = await _db.Orders.FindAsync(request.OrderId);
            
            if (order == null)
            {
                Log.Information($"Order {request.OrderId} could not be found.");
                
                return new Result<Courier> {Reason = ReasonType.OrderNotFound, IsSuccessful = false};
            }

            order.CourierId = null;
            order.StatusTimestamp = DateTime.Now;

            _db.Update(order);
            
            var changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
            {
                Log.Information($"Courier {request.CourierId} was not updated.");
                
                return new Result<Courier> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
            
            var address = await _db.Addresses.FindAsync(courier.AddressId);

            Log.Information($"Order {request.OrderId} and courier {request.CourierId} information was updated.");
                
            return new Result<Courier> {ChangeCount = changes, Value = MapEntity(courier, address), IsSuccessful = true};
        }

        public async Task<Result<Order>> PickUpOrder(CourierDispatchRequest request)
        {
            var restaurant = await _db.Restaurants.FindAsync(request.RestaurantId);
            
            if (restaurant == null || !restaurant.IsActive)
            {
                Log.Information($"Restaurant {request.RestaurantId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.RestaurantNotFound, IsSuccessful = false};
            }
            
            if (!restaurant.IsOpen)
            {
                Log.Information($"Restaurant {request.RestaurantId} is not open.");
                
                return new Result<Order> {Reason = ReasonType.RestaurantNotOpen, IsSuccessful = false};
            }

            var courier = await _db.Couriers.FindAsync(request.CourierId);
            
            if (courier == null)
            {
                Log.Information($"Courier {request.CourierId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};
            }
            
            courier.Status = (int)CourierStatus.PickedUpOrder;
            courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(courier);
            
            var order = await _db.Orders.FindAsync(request.OrderId);
            
            if (order == null)
            {
                Log.Information($"Order {request.OrderId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.OrderNotFound, IsSuccessful = false};
            }

            order.CourierId = courier.CourierId;
            order.Status = (int) OrderStatus.Delivering;
            order.StatusTimestamp = DateTime.Now;

            _db.Update(order);
            
            int changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
            {
                Log.Information($"Order {request.OrderId} was not updated.");
                
                return new Result<Order> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
            
            Log.Information($"Order {request.OrderId} and courier {request.CourierId} information was updated.");
                
            return new Result<Order> {ChangeCount = changes, Value = MapEntity(courier, order), IsSuccessful = true};
        }

        public async Task<Result<Order>> Deliver(CourierDispatchRequest request)
        {
            var courier = await _db.Couriers.FindAsync(request.CourierId);
            
            if (courier == null)
            {
                Log.Information($"Courier {request.CourierId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};
            }
            
            courier.Status = (int)CourierStatus.DeliveredOrder;
            courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(courier);
            
            var order = await _db.Orders.FindAsync(request.OrderId);
            
            if (order == null)
            {
                Log.Information($"Order {request.OrderId} could not be found.");
                
                return new Result<Order> {Reason = ReasonType.OrderNotFound, IsSuccessful = false};
            }

            order.CourierId ??= courier.CourierId;
            order.Status = (int) OrderStatus.Delivered;
            order.StatusTimestamp = DateTime.Now;

            _db.Update(order);
            
            int changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
            {
                Log.Information($"Order {request.OrderId} was not updated.");
                
                return new Result<Order> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
            
            Log.Information($"Order {request.OrderId} and courier {request.CourierId} information was updated.");
                
            return new Result<Order> {ChangeCount = changes, Value = MapEntity(courier, order), IsSuccessful = true};
        }

        public async Task<Result<Courier>> ChangeStatus(CourierStatusChangeRequest request)
        {
            var courier = await _db.Couriers.FindAsync(request.CourierId);
            
            if (courier == null)
            {
                Log.Information($"Courier {request.CourierId} could not be found.");
                
                return new Result<Courier> {Reason = ReasonType.CourierNotFound, IsSuccessful = false};
            }
            
            courier.Status = (int)request.Status;
            courier.StatusTimestamp = DateTime.Now;
            
            _db.Update(courier);
            
            int changes = await _db.SaveChangesAsync();
            
            if (changes <= 0)
            {
                Log.Information($"Courier {request.CourierId} status was not updated.");
                
                return new Result<Courier> {Reason = ReasonType.DatabaseError, ChangeCount = changes, IsSuccessful = false};
            }
            
            var address = await _db.Addresses.FindAsync(courier.AddressId);

            Log.Information($"Courier {request.CourierId} status was updated.");
                
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
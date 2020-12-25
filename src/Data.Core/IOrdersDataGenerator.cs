namespace Data.Core
{
    using System.Collections.Generic;
    using Model;

    public interface IOrdersDataGenerator
    {
        List<Region> Regions { get; }
        
        List<StorageTemperature> Temperatures { get; }
        
        List<Restaurant> Restaurants { get; }
        
        List<Menu> Menus { get; }
        
        List<MenuItem> MenuItems { get; }
        
        List<Customer> Customers { get; }
        
        List<Shelf> Shelves { get; }
        
        List<Courier> Couriers { get; }
        
        List<Order> Orders { get; }
        
        List<OrderItem> OrderItems { get; }
    }
}
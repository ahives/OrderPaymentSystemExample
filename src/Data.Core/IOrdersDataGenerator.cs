namespace Data.Core
{
    using System.Collections.Generic;
    using Model;

    public interface IOrdersDataGenerator
    {
        List<RegionEntity> Regions { get; }
        
        List<StorageTemperatureEntity> Temperatures { get; }
        
        List<RestaurantEntity> Restaurants { get; }
        
        List<MenuEntity> Menus { get; }
        
        List<MenuItemEntity> MenuItems { get; }
        
        List<CustomerEntity> Customers { get; }
        
        List<ShelfEntity> Shelves { get; }
        
        List<CourierEntity> Couriers { get; }
        
        List<OrderEntity> Orders { get; }
        
        List<OrderItem> OrderItems { get; }
    }
}
namespace DatabaseSeederConsole
{
    using System.Collections.Generic;
    using Data.Core.Model;

    public interface IOrdersDataGenerator
    {
        List<RegionEntity> Regions { get; }
        
        List<TemperatureEntity> Temperatures { get; }
        
        List<RestaurantEntity> Restaurants { get; }
        
        List<MenuEntity> Menus { get; }
        
        List<MenuItemEntity> MenuItems { get; }
        
        List<CustomerEntity> Customers { get; }
        
        List<ShelfEntity> Shelves { get; }
        
        List<CourierEntity> Couriers { get; }
        
        List<OrderEntity> Orders { get; }
        
        List<OrderItemEntity> OrderItems { get; }
        
        List<AddressEntity> Addresses { get; }
        
        List<IngredientEntity> Ingredients { get; }
        
        List<InventoryItemEntity> InventoryItems { get; }
        
        List<MenuItemIngredientEntity> MenuItemIngredients { get; }
    }
}
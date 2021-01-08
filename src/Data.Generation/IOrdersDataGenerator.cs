namespace Data.Generation
{
    using System.Collections.Generic;
    using Core.Model;

    public interface IOrdersDataGenerator
    {
        IReadOnlyList<RegionEntity> Regions { get; }
        
        IReadOnlyList<TemperatureEntity> Temperatures { get; }
        
        IReadOnlyList<RestaurantEntity> Restaurants { get; }
        
        IReadOnlyList<MenuEntity> Menus { get; }
        
        IReadOnlyList<MenuItemEntity> MenuItems { get; }
        
        IReadOnlyList<CustomerEntity> Customers { get; }
        
        IReadOnlyList<ShelfEntity> Shelves { get; }
        
        IReadOnlyList<CourierEntity> Couriers { get; }
        
        IReadOnlyList<OrderEntity> Orders { get; }
        
        IReadOnlyList<OrderItemEntity> OrderItems { get; }
        
        IReadOnlyList<AddressEntity> Addresses { get; }
        
        IReadOnlyList<IngredientEntity> Ingredients { get; }
        
        IReadOnlyList<InventoryItemEntity> InventoryItems { get; }
        
        IReadOnlyList<MenuItemIngredientEntity> MenuItemIngredients { get; }
    }
}
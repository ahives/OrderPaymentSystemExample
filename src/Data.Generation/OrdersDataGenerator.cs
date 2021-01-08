namespace Data.Generation
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Model;

    public class OrdersDataGenerator :
        DataSeeder,
        IOrdersDataGenerator
    {
        public IReadOnlyList<RegionEntity> Regions { get; }
        public IReadOnlyList<TemperatureEntity> Temperatures { get; }
        public IReadOnlyList<RestaurantEntity> Restaurants { get; }
        public IReadOnlyList<MenuEntity> Menus { get; }
        public IReadOnlyList<MenuItemEntity> MenuItems { get; }
        public IReadOnlyList<CustomerEntity> Customers { get; }
        public IReadOnlyList<ShelfEntity> Shelves { get; }
        public IReadOnlyList<CourierEntity> Couriers { get; }
        public IReadOnlyList<OrderEntity> Orders { get; }
        public IReadOnlyList<OrderItemEntity> OrderItems { get; }
        public IReadOnlyList<AddressEntity> Addresses { get; }
        public IReadOnlyList<IngredientEntity> Ingredients { get; }
        public IReadOnlyList<InventoryItemEntity> InventoryItems { get; }
        public IReadOnlyList<MenuItemIngredientEntity> MenuItemIngredients { get; }

        public OrdersDataGenerator()
        {
            Regions = GetRegionFaker(GetRegions()).Generate(5);
            Temperatures = GetTemperatureFaker(GetTemperatures()).Generate(4);
            Addresses = GetAddressFaker(GetCities(), GetStreets(), GetZipCodes(), Regions.Select(x => x.RegionId)).Generate(20);
            Restaurants = GetRestaurantFaker(GetRestaurants(), Addresses.Select(x => x.AddressId)).Generate(10);
            Menus = GetMenuFaker(GetMenus(), Restaurants.Select(x => x.RestaurantId)).Generate(3);
            MenuItems = GetMenuItemFaker(GetMenuItems(), Temperatures.Select(x => x.TemperatureId),
                Menus.Select(x => x.MenuId)).Generate(20);
            Customers = GetCustomerFaker(GetFirstNames(), GetLastNames(), Addresses.Select(x => x.AddressId)).Generate(5);
            Ingredients = GetIngredientFaker(GetIngredients(), Temperatures.Select(x => x.TemperatureId)).Generate(15);
            InventoryItems = GetInventoryItemFaker(Ingredients.Select(x => x.IngredientId),
                Restaurants.Select(x => x.RestaurantId)).Generate(20);
            MenuItemIngredients = GetMenuItemIngredientFaker(MenuItems.Select(x => x.MenuItemId),
                Ingredients.Select(x => x.IngredientId)).Generate(50);

            var shelves = new List<ShelfEntity>();

            for (int i = 0; i < 3; i++)
                shelves.AddRange(GetShelfFaker(Temperatures.Select(x => x.TemperatureId),
                    Restaurants.Select(x => x.RestaurantId)).Generate(1));
            shelves.AddRange(GetShelfFaker(Temperatures.Select(x => x.TemperatureId),
                Restaurants.Select(x => x.RestaurantId), true).Generate(1));
            
            Shelves = shelves;

            Couriers = GetCourierFaker(GetFirstNames(), GetLastNames(), Addresses.Select(x => x.AddressId)).Generate(20);
            Orders = GetOrderFaker(Customers.Select(x => x.CustomerId),
                Restaurants.Select(x => x.RestaurantId), Addresses.Select(x => x.AddressId),
                Couriers.Select(x => x.CourierId)).Generate(10);
            OrderItems = GetOrderItemFaker(MenuItems.Select(x => x.MenuItemId),
                Orders.Select(x => x.OrderId), Shelves.Select(x => x.ShelfId)).Generate(20);
        }
    }
}
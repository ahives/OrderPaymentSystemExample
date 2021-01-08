namespace Data.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bogus;
    using Core.Model;
    using MassTransit;

    public class DataSeeder
    {
        protected virtual IEnumerable<string> GetIngredients()
        {
            yield return "Chocolate";
            yield return "Ice";
            yield return "American Cheese";
            yield return "Steak";
            yield return "Rice";
            yield return "Dough";
            yield return "Hamburger Patty";
            yield return "Noodles";
            yield return "Chicken";
            yield return "Milk";
            yield return "Orange";
            yield return "Lettuce";
            yield return "Tomato";
            yield return "Lemon";
            yield return "Onion";
            yield return "Potato";
            yield return "Flower Tortilla";
            yield return "Cookie Dough";
            yield return "Bacon";
            yield return "Apple Juice";
            yield return "Hamburger Bun";
            yield return "Egg";
        }

        protected virtual IEnumerable<string> GetMenus()
        {
            yield return "Breakfast";
            yield return "Lunch";
            yield return "Dinner";
            yield return "Brunch";
            yield return "Desert";
        }

        protected virtual IEnumerable<string> GetRegions()
        {
            yield return "California";
            yield return "New York";
            yield return "Georgia";
            yield return "Washington";
            yield return "Oregon";
            yield return "Texas";
        }

        protected virtual IEnumerable<string> GetMenuItems()
        {
            yield return "Hot Chocolate";
            yield return "Smoothie";
            yield return "Cheese Pizza";
            yield return "Steak";
            yield return "Rice and Gravy";
            yield return "Philly Cheese Steak";
            yield return "Hamburger";
            yield return "Lasagna";
            yield return "Soup";
            yield return "Milk";
            yield return "Orange Juice";
            yield return "House Salad";
            yield return "Fruit Punch";
            yield return "Mac & Cheese";
            yield return "Ice Cream";
            yield return "French Fries";
            yield return "Bacon, Egg, and Cheese Burrito";
            yield return "Desert";
        }

        protected virtual IEnumerable<string> GetZipCodes()
        {
            yield return "93483";
            yield return "92301";
            yield return "83324";
            yield return "93924";
            yield return "82474";
            yield return "69843";
            yield return "73934";
        }

        protected virtual IEnumerable<string> GetFirstNames()
        {
            yield return "Albert";
            yield return "Christy";
            yield return "Jose";
            yield return "Stephen";
            yield return "Michael";
            yield return "Sarah";
            yield return "Mia";
        }

        protected virtual IEnumerable<string> GetLastNames()
        {
            yield return "Jones";
            yield return "Lacey";
            yield return "Jordan";
            yield return "Curry";
            yield return "Wiseman";
            yield return "Chavez";
            yield return "Williams";
        }

        protected virtual IEnumerable<string> GetRestaurants()
        {
            yield return "Breakfast & Stuff";
            yield return "All Day";
            yield return "Dinner & Things";
            yield return "Groovy Smoothie";
            yield return "Big Al's";
            yield return "Lunch R Us";
        }

        protected virtual IEnumerable<string> GetTemperatures()
        {
            yield return "Hot";
            yield return "Cold";
            yield return "Frozen";
            yield return "Warm";
            yield return "Overflow";
        }

        protected virtual IEnumerable<string> GetStreets()
        {
            yield return "123 E. 12th St.";
            yield return "425 Broadway Ave.";
            yield return "948 West St.";
            yield return "99 California St.";
            yield return "123 E. 14th Blvd.";
            yield return "939 Crenshaw Blvd.";
            yield return "294 E. Cotati Blvd.";
        }

        protected virtual IEnumerable<string> GetCities()
        {
            yield return "Oakland";
            yield return "Atlanta";
            yield return "Seattle";
            yield return "Portland";
            yield return "Los Angeles";
            yield return "San Francisco";
            yield return "New York";
            yield return "Chicago";
            yield return "New Orleans";
            yield return "Philadelphia";
        }

        protected virtual Faker<RegionEntity> GetRegionFaker(IEnumerable<string> regions)
        {
            int i = 0;
            
            var faker = new Faker<RegionEntity>()
                .StrictMode(true)
                .RuleFor(x => x.RegionId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s =>
                {
                    if (i < regions.Count())
                        return regions.ElementAt(i++);

                    i++;
                    return null;
                })
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<AddressEntity> GetAddressFaker(IEnumerable<string> cities, IEnumerable<string> streets, IEnumerable<string> zipCodes, IEnumerable<Guid> regions)
        {
            var faker = new Faker<AddressEntity>()
                .StrictMode(true)
                .Ignore(x => x.Region)
                .RuleFor(x => x.AddressId, s => NewId.NextGuid())
                .RuleFor(x => x.City, s => s.PickRandom(cities))
                .RuleFor(x => x.Street, s => s.PickRandom(streets))
                .RuleFor(x => x.RegionId, s => s.PickRandom(regions))
                .RuleFor(x => x.ZipCode, s => s.PickRandom(zipCodes))
                .RuleFor(x => x.CreationTimestamp, DateTime.Now);

            return faker;
        }

        protected virtual Faker<CourierEntity> GetCourierFaker(IEnumerable<string> firstNames, IEnumerable<string> lastNames, IEnumerable<Guid> addresses)
        {
            var faker = new Faker<CourierEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.CourierId, s => NewId.NextGuid())
                .RuleFor(x => x.Status, s => 0)
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.FirstName, s => s.PickRandom(firstNames))
                .RuleFor(x => x.LastName, s => s.PickRandom(lastNames))
                .RuleFor(x => x.IsActive, s => true)
                .RuleFor(x => x.AddressId, s => s.PickRandom(addresses))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<InventoryItemEntity> GetInventoryItemFaker(IEnumerable<Guid> ingredients, IEnumerable<Guid> restaurants)
        {
            var faker = new Faker<InventoryItemEntity>()
                .StrictMode(true)
                .Ignore(x => x.Ingredient)
                .Ignore(x => x.Restaurant)
                .RuleFor(x => x.InventoryItemId, s => NewId.NextGuid())
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(restaurants))
                .RuleFor(x => x.IngredientId, s => s.PickRandom(ingredients))
                .RuleFor(x => x.QuantityOnHand, s => s.Random.Decimal(0M, 10.0M))
                .RuleFor(x => x.ReplenishmentThreshold, s => s.Random.Decimal(5.0M, 7.0M))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<MenuItemIngredientEntity> GetMenuItemIngredientFaker(IEnumerable<Guid> menuItems, IEnumerable<Guid> ingredients)
        {
            var faker = new Faker<MenuItemIngredientEntity>()
                .StrictMode(true)
                .Ignore(x => x.Ingredient)
                .Ignore(x => x.MenuItem)
                .RuleFor(x => x.MenuItemIngredientId, s => NewId.NextGuid())
                .RuleFor(x => x.MenuItemId, s => s.PickRandom(menuItems))
                .RuleFor(x => x.IngredientId, s => s.PickRandom(ingredients))
                .RuleFor(x => x.QuantityToUse, s => s.Random.Decimal(5.0M, 10.0M))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<IngredientEntity> GetIngredientFaker(IEnumerable<string> ingredients, IEnumerable<Guid> temperatures)
        {
            var faker = new Faker<IngredientEntity>()
                .StrictMode(true)
                .Ignore(x => x.Temperature)
                .RuleFor(x => x.IngredientId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(ingredients))
                .RuleFor(x => x.TemperatureId, s => s.PickRandom(temperatures))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<OrderEntity> GetOrderFaker(IEnumerable<Guid> customers, IEnumerable<Guid> restaurants, IEnumerable<Guid> addresses, IEnumerable<Guid> couriers)
        {
            var faker = new Faker<OrderEntity>()
                .StrictMode(true)
                .Ignore(x => x.Customer)
                .Ignore(x => x.Courier)
                .Ignore(x => x.Restaurant)
                .Ignore(x => x.Address)
                .RuleFor(x => x.OrderId, s => NewId.NextGuid())
                .RuleFor(x => x.CustomerPickup, s => false)
                .RuleFor(x => x.Status, s => s.Random.Int(0, 2))
                .RuleFor(x => x.CustomerId, s => s.PickRandom(customers))
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(restaurants))
                .RuleFor(x => x.AddressId, s => s.PickRandom(addresses))
                .RuleFor(x => x.CourierId, s => s.PickRandom(couriers))
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<CustomerEntity> GetCustomerFaker(IEnumerable<string> firstNames, IEnumerable<string> lastNames, IEnumerable<Guid> addresses)
        {
            var faker = new Faker<CustomerEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.CustomerId, s => NewId.NextGuid())
                .RuleFor(x => x.FirstName, s => s.PickRandom(firstNames))
                .RuleFor(x => x.LastName, s => s.PickRandom(lastNames))
                .RuleFor(x => x.AddressId, s => s.PickRandom(addresses))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<RestaurantEntity> GetRestaurantFaker(IEnumerable<string> restaurants, IEnumerable<Guid> addresses)
        {
            var faker = new Faker<RestaurantEntity>()
                .StrictMode(true)
                .Ignore(x => x.Address)
                .RuleFor(x => x.RestaurantId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(restaurants))
                .RuleFor(x => x.IsActive, s => true)
                .RuleFor(x => x.IsOpen, s => true)
                .RuleFor(x => x.AddressId, s => s.PickRandom(addresses))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<OrderItemEntity> GetOrderItemFaker(IEnumerable<Guid> menuItems, IEnumerable<Guid> orders, IEnumerable<Guid> shelves)
        {
            var faker = new Faker<OrderItemEntity>()
                .StrictMode(true)
                .Ignore(x => x.Order)
                .Ignore(x => x.MenuItem)
                .Ignore(x => x.Shelf)
                .RuleFor(x => x.OrderItemId, s => NewId.NextGuid())
                .RuleFor(x => x.Status, s => s.Random.Int(0, 3))
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.MenuItemId, s => s.PickRandom(menuItems))
                .RuleFor(x => x.OrderId, s => s.PickRandom(orders))
                .RuleFor(x => x.ShelfId, s => s.PickRandom(shelves))
                .RuleFor(x => x.TimePrepared, s => DateTime.Now)
                .RuleFor(x => x.ExpiryTimestamp, s => DateTime.Now)
                .RuleFor(x => x.StatusTimestamp, s => DateTime.Now)
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.SpecialInstructions, s => s.PickRandom(string.Empty, "Some random instructions"))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<ShelfEntity> GetShelfFaker(IEnumerable<Guid> temperatures, IEnumerable<Guid> restaurants, bool isOverflow = false)
        {
            var faker = new Faker<ShelfEntity>()
                .StrictMode(true)
                .Ignore(x => x.Temperature)
                .Ignore(x => x.Restaurant)
                .RuleFor(x => x.ShelfId, s => NewId.NextGuid())
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(restaurants))
                .RuleFor(x => x.IsOverflow, s => isOverflow)
                .RuleFor(x => x.Capacity, s => s.PickRandom(5, 10, 15, 20))
                .RuleFor(x => x.Name, s => s.Random.Replace("##-????"))
                .RuleFor(x => x.DecayRate, s => s.Random.Decimal(10M, 20M))
                .RuleFor(x => x.TemperatureId, s => s.PickRandom(temperatures))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<MenuItemEntity> GetMenuItemFaker(IEnumerable<string> menuItems, IEnumerable<Guid> temperatures, IEnumerable<Guid> menus)
        {
            var faker = new Faker<MenuItemEntity>()
                .StrictMode(true)
                .Ignore(x => x.Menu)
                .Ignore(x => x.Temperature)
                .RuleFor(x => x.MenuItemId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(menuItems))
                .RuleFor(x => x.Price, s => s.Random.Decimal(1, 25))
                .RuleFor(x => x.ShelfLife, s => s.Random.Decimal(50M, 100M))
                .RuleFor(x => x.IsActive, s => true)
                .RuleFor(x => x.TemperatureId, s => s.PickRandom(temperatures))
                .RuleFor(x => x.MenuId, s => s.PickRandom(menus))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<MenuEntity> GetMenuFaker(IEnumerable<string> menus, IEnumerable<Guid> restaurants)
        {
            var faker = new Faker<MenuEntity>()
                .StrictMode(true)
                .Ignore(x => x.Restaurant)
                .RuleFor(x => x.MenuId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s => s.PickRandom(menus))
                .RuleFor(x => x.RestaurantId, s => s.PickRandom(restaurants))
                .RuleFor(x => x.IsActive, s => s.PickRandom(true, false))
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }

        protected virtual Faker<TemperatureEntity> GetTemperatureFaker(IEnumerable<string> temperatures)
        {
            int i = 0;

            var faker = new Faker<TemperatureEntity>()
                .StrictMode(true)
                .RuleFor(x => x.TemperatureId, s => NewId.NextGuid())
                .RuleFor(x => x.Name, s =>
                {
                    if (i < temperatures.Count())
                        return temperatures.ElementAt(i++);

                    i++;
                    return null;
                })
                .RuleFor(x => x.CreationTimestamp, s => DateTime.Now);

            return faker;
        }
    }
}
namespace Service.Grpc.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.Core;
    using Data.Core.Model;
    using Model;

    public class LowInventoryDetector :
        ILowInventoryDetector
    {
        readonly OrdersDbContext _db;

        public LowInventoryDetector(OrdersDbContext db)
        {
            _db = db;
        }

        public async IAsyncEnumerable<Result<Inventory>> FindAll()
        {
            var inventories = Enumerable.GroupJoin(_db.InventoryItems
                        .Where(x => x.QuantityOnHand <= x.ReplenishmentThreshold)
                        .ToList(), _db.InventoryItems,
                    inventory => inventory.RestaurantId,
                    menuItem => menuItem.IngredientId,
                    MapInventory);
            // var inventories = _db.InventoryItems
            //     .AsEnumerable()
            //     .Where(x => x.QuantityOnHand <= x.ReplenishmentThreshold)
            //     .GroupJoin(
            //         _db.InventoryItems,
            //         inventory => inventory.RestaurantId,
            //         menuItem => menuItem.IngredientId,
            //         MapInventory);

            foreach (var inventory in inventories)
                yield return new Result<Inventory> {Value = inventory, Reason = ReasonType.None};
        }

        InventoryItem MapInventoryItem(InventoryItemEntity entity) =>
            new()
            {
                InventoryItemId = entity.InventoryItemId,
                RestaurantId = entity.RestaurantId,
                IngredientId = entity.IngredientId,
                QuantityOnHand = entity.QuantityOnHand,
                ReplenishmentThreshold = entity.ReplenishmentThreshold
            };

        Inventory MapInventory(InventoryItemEntity entity, IEnumerable<InventoryItemEntity> inventoryItemEntities) =>
            new()
            {
                RestaurantId = entity.RestaurantId,
                Items = inventoryItemEntities.Select(x => MapInventoryItem(x)).ToList()
            };
    }
}
namespace RestaurantService.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Quartz;
    using Serilog;
    using Service.Grpc.Core;
    using Service.Grpc.Core.Model;

    public class LowInventoryDetectorJob :
        IJob
    {
        readonly ILowInventoryDetector _detector;

        public LowInventoryDetectorJob(ILowInventoryDetector detector)
        {
            _detector = detector;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            IAsyncEnumerable<Result<Inventory>> results = _detector.FindAll();

            await foreach (var result in results)
            {
                Log.Information($"RestaurantId: {result.Value.RestaurantId}");
                foreach (var inventoryItem in result.Value.Items)
                {
                    Log.Information($"IngredientId: {inventoryItem.IngredientId}, QuantityOnHand: {inventoryItem.QuantityOnHand}, ReplenishmentThreshold: {inventoryItem.ReplenishmentThreshold}");
                }
            }
        }
    }
}
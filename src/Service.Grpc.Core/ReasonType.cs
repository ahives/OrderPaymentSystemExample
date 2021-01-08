namespace Service.Grpc.Core
{
    public enum ReasonType
    {
        RestaurantNotOpen,
        RestaurantNotActive,
        CourierNotFound,
        CourierNotActive,
        CourierNotAvailable,
        MovedToShelf,
        Receipt,
        ExpiredOrder,
        None
    }
}
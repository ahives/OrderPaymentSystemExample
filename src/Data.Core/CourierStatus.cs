namespace Data.Core
{
    public enum CourierStatus
    {
        Idle = 0,
        Requested = 1,
        Dispatched = 2,
        Confirmed = 3,
        Declined = 4,
        EnRouteToRestaurant = 5,
        ArrivedAtRestaurant = 6,
        PickedUpOrder = 7,
        EnRouteToCustomer = 8,
        ArrivedAtCustomer = 9,
        DeliveredOrder = 10
    }
}
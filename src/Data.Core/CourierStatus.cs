namespace Data.Core
{
    public enum CourierStatus
    {
        Idle = 0,
        Dispatched = 1,
        Confirmed = 2,
        PickedUpOrder = 3,
        DeliveredOrder = 4,
        EnRouteToRestaurant = 5,
        ArrivedAtRestaurant = 6,
        EnRouteToCustomer = 7,
        ArrivedAtCustomer = 8,
        Declined = 9
    }
}
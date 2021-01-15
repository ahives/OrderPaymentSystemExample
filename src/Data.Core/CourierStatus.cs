namespace Data.Core
{
    public enum CourierStatus
    {
        Idle = 0,
        DispatchRequested = 1,
        Dispatched = 2,
        DispatchConfirmed = 3,
        DispatchDeclined = 4,
        EnRouteToRestaurant = 5,
        ArrivedAtRestaurant = 6,
        PickedUpOrder = 7,
        EnRouteToCustomer = 8,
        ArrivedAtCustomer = 9,
        DeliveredOrder = 10,
        DispatchCanceled = 11
    }
}
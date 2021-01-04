namespace Data.Core
{
    public enum CourierStatus
    {
        Idle = 0,
        Dispatched = 1,
        Confirmed = 2,
        PickedUpOrder = 3,
        DeliveredOrder = 4,
        EnRoute = 5,
        Declined = 6
    }
}
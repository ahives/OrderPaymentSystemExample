namespace Data.Core
{
    public static class CourierStatus
    {
        public static int Idle = 0;

        public static int Dispatched = 1;

        public static int Confirmed = 2;

        public static int PickedUpOrder = 3;

        public static int DeliveredOrder = 4;
    }
}
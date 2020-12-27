namespace Data.Core
{
    public static class OrderItemStatus
    {
        public static int New = 0;
        
        public static int Prepared = 1;
        
        public static int Discarded = 2;
        
        public static int Delivered = 3;
        
        public static int Expired = 4;
        
        public static int Canceled = 5;
        
        public static int NptPrepared = 6;
    }
}
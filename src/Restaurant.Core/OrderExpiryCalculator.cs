namespace Restaurant.Core
{
    public class OrderExpiryCalculator :
        IOrderExpiryCalculator
    {
        public bool CalcExpiry(ExpiryCriteria criteria)
        {
            return true;
        }
    }
}
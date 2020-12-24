namespace Restaurant.Core
{
    public interface IOrderExpiryCalculator
    {
        bool CalcExpiry(ExpiryCriteria criteria);
    }
}
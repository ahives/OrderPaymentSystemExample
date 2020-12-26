namespace Services.Core
{
    public interface IOrderExpiryCalculator
    {
        bool CalcExpiry(ExpiryCriteria criteria);
    }
}
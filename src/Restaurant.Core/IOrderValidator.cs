namespace Restaurant.Core
{
    public interface IOrderValidator
    {
        bool Validate(ValidateOrder message);
    }
}
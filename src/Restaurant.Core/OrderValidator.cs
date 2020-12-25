namespace Restaurant.Core
{
    public class OrderValidator :
        IOrderValidator
    {
        public bool Validate(ValidateOrder message) => true;
    }
}